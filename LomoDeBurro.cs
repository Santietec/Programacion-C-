using System.Collections.Generic;
using System;
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
        // Visualizar el lomo de burro en el editor
        Gizmos.color = EstaActivo ? Color.yellow : Color.gray;
        Vector3 size = new Vector3(2f, altura, longitud);
        Gizmos.DrawWireCube(transform.position + Vector3.up * (altura/2), size);
    }
}

// Gestor de lomos de burro en la carretera
public class GestorLomosDeBurro : MonoBehaviour
{
    [SerializeField] private float margenDeteccion = 5.0f;
    private List<LomoDeBurro> _lomosDeBurro = new List<LomoDeBurro>();

    private void Awake()
    {
        // Encontrar todos los lomos de burro en la escena
        _lomosDeBurro.AddRange(FindObjectsOfType<LomoDeBurro>());
    }

    public void RegistrarLomoDeBurro(LomoDeBurro lomo)
    {
        if (!_lomosDeBurro.Contains(lomo))
        {
            _lomosDeBurro.Add(lomo);
        }
    }

    public void DesregistrarLomoDeBurro(LomoDeBurro lomo)
    {
        _lomosDeBurro.Remove(lomo);
    }

    public LomoDeBurro ObtenerLomoDeBurroCercano(float posicionVehiculo)
    {
        LomoDeBurro lomoMasCercano = null;
        float distanciaMinima = margenDeteccion;

        foreach (var lomo in _lomosDeBurro)
        {
            if (!lomo.EstaActivo) continue;

            float distancia = Mathf.Abs(lomo.Posicion - posicionVehiculo);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                lomoMasCercano = lomo;
            }
        }

        return lomoMasCercano;
    }

    private void OnDrawGizmos()
    {
        // Visualizar el área de detección en el editor
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, margenDeteccion);
    }
}

// Detector de lomos de burro para el vehículo
public class DetectorLomoDeBurro : MonoBehaviour
{
    [SerializeField] private GestorLomosDeBurro gestorLomos;
    [SerializeField] private float factorReduccionVelocidad = 1f;
    [SerializeField] private float umbralSacudida = 5.0f;
    
    private Vehiculo _vehiculo;

    public event Action<float> LomoDeBurroDetectado;

    private void Awake()
    {
        _vehiculo = GetComponent<Vehiculo>();
        if (_vehiculo == null)
        {
            Debug.LogError("No se encontró el componente Vehiculo en el mismo GameObject");
        }

        if (gestorLomos == null)
        {
            gestorLomos = FindObjectOfType<GestorLomosDeBurro>();
            if (gestorLomos == null)
            {
                Debug.LogError("No se encontró el GestorLomosDeBurro en la escena");
            }
        }
    }

    private void Update()
    {
        if (_vehiculo == null || gestorLomos == null) return;

        var lomoCercano = gestorLomos.ObtenerLomoDeBurroCercano(_vehiculo.Posicion);

        if (lomoCercano != null)
        {
            float impacto = lomoCercano.CalcularEfecto(_vehiculo.VelocidadActual);
            LomoDeBurroDetectado?.Invoke(impacto);
            AplicarEfectoAlVehiculo(impacto);
        }
    }

    private void AplicarEfectoAlVehiculo(float impacto)
    {
        // Reducir la velocidad del vehículo
        _vehiculo.VelocidadActual = Mathf.Max(0, _vehiculo.VelocidadActual - (impacto * factorReduccionVelocidad));

        // Simular la sacudida del vehículo
        if (impacto > umbralSacudida)
        {
            // Aplicar una fuerza hacia arriba al vehículo
            if (TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(Vector3.up * impacto, ForceMode.Impulse);
            }

            // Reproducir efectos de sonido o partículas si están configurados
            if (TryGetComponent<AudioSource>(out var audioSource))
            {
                audioSource.Play();
            }
        }
    }
}
