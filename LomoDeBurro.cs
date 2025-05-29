using UnityEngine;

// Clase principal para el lomo de burro
public class LomoDeBurro : MonoBehaviour
{
    [SerializeField] private float altura = 0.2f;
    [SerializeField] private float longitud = 2f;
    [SerializeField] private bool estaActivo = true;

    public float Altura => altura;
    public float Longitud => longitud;
    public float Posicion => transform.position.z;
    public bool EstaActivo { get => estaActivo; set => estaActivo = value; }

    private void OnValidate()
    {
        // Asegurarse de que los valores sean positivos
        altura = Mathf.Max(0, altura);
        longitud = Mathf.Max(0.1f, longitud);
    }

    // Método para calcular el efecto del lomo de burro en el vehículo
    public float CalcularEfecto(float velocidadVehiculo)
    {
        if (!EstaActivo)
            return 0f;

        // A mayor velocidad, mayor será el impacto del lomo de burro
        float factorImpacto = velocidadVehiculo > 30 ? 0.2f : 0.1f;
        return velocidadVehiculo * factorImpacto * altura;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = EstaActivo ? Color.yellow : Color.gray;

        // Intentar usar el tamaño real del modelo
        Bounds bounds = new Bounds(transform.position, new Vector3(2f, altura, longitud));

        // Si hay un Renderer, usar su bounds
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            bounds = renderer.bounds;
        }
        // Si no, pero hay un Collider, usar su bounds
        else if (GetComponent<Collider>() != null)
        {
            bounds = GetComponent<Collider>().bounds;
        }

        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
