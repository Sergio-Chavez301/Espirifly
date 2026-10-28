using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SoulController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    public float fuerzaSalto = 13f;
    public LayerMask capaSuelo;

    [Header("Salto en pared")]
    public float velocidadDeslizPared = 2f;
    public Vector2 saltoPared = new Vector2(7f, 12f);
    public float bloqueoPared = 0.2f;

    [Header("Dash aéreo")]
    public float velocidadDash = 16f;
    public float duracionDash = 0.15f;

    [Header("Caída al vacío")]
    public float alturaMinima = -5f;

    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer sr;
    Vector3 inicio;
    float gravedadBase;
    float bloqueoTimer, dashTimer;
    float dir = 1f;
    bool dashDisponible = true, haciendoDash;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        gravedadBase = rb.gravityScale;
        inicio = transform.position;
    }

    void Update()
    {
        var k = Keyboard.current;
        if (k == null) return;

        float h = 0f;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) h -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) h += 1f;
        bool saltar = k.spaceKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame;
        bool soltarSalto = k.spaceKey.wasReleasedThisFrame || k.wKey.wasReleasedThisFrame || k.upArrowKey.wasReleasedThisFrame;
        bool dash = k.leftShiftKey.wasPressedThisFrame || k.jKey.wasPressedThisFrame || k.xKey.wasPressedThisFrame;

        if (transform.position.y < alturaMinima) { Reaparecer(); return; }

        Bounds b = col.bounds;
        bool enSuelo = Physics2D.OverlapBox(new Vector2(b.center.x, b.min.y - 0.05f), new Vector2(b.size.x * 0.9f, 0.1f), 0f, capaSuelo);
        bool paredIzq = Physics2D.OverlapBox(new Vector2(b.min.x - 0.05f, b.center.y), new Vector2(0.1f, b.size.y * 0.8f), 0f, capaSuelo);
        bool paredDer = Physics2D.OverlapBox(new Vector2(b.max.x + 0.05f, b.center.y), new Vector2(0.1f, b.size.y * 0.8f), 0f, capaSuelo);

        if (enSuelo && !haciendoDash) dashDisponible = true;

        // Dash en curso
        if (haciendoDash)
        {
            dashTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(dir * velocidadDash, 0f);
            if (dashTimer <= 0f) TerminarDash();
            return;
        }

        if (h != 0f) { dir = Mathf.Sign(h); if (sr) sr.flipX = dir < 0f; }

        // Iniciar dash (solo en el aire y una vez por salto)
        if (dash && !enSuelo && dashDisponible)
        {
            dashDisponible = false;
            haciendoDash = true;
            dashTimer = duracionDash;
            rb.gravityScale = 0f;
            return;
        }

        Vector2 v = rb.linearVelocity;

        // Movimiento horizontal
        if (bloqueoTimer > 0f) bloqueoTimer -= Time.deltaTime;
        else v.x = h * velocidad;

        // Deslizar por la pared
        bool pegadoPared = !enSuelo && ((paredIzq && h < 0f) || (paredDer && h > 0f));
        if (pegadoPared && v.y < 0f) v.y = Mathf.Max(v.y, -velocidadDeslizPared);

        // Salto
        if (saltar)
        {
            if (enSuelo) v.y = fuerzaSalto;
            else if (paredIzq || paredDer)
            {
                v = new Vector2((paredIzq ? 1f : -1f) * saltoPared.x, saltoPared.y);
                bloqueoTimer = bloqueoPared;
                dashDisponible = true;
            }
        }
        // Salto corto si se suelta la tecla
        if (soltarSalto && v.y > 0f) v.y *= 0.5f;

        rb.linearVelocity = v;
    }

    void TerminarDash()
    {
        haciendoDash = false;
        rb.gravityScale = gravedadBase;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.3f, 0f);
    }

    public void Reaparecer()
    {
        TerminarDash();
        transform.position = inicio;
        rb.linearVelocity = Vector2.zero;
        dashDisponible = true;
    }
}