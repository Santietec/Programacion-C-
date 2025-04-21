using System.Collections.Generic;
using System;
using UnityEngine;

// Clase principal para el lomo de burro
public class LomoDeBurro
{
    public float Altura { get; private set; }
    public float Longitud { get; private set; }
    public float Posicion { get; private set; }
    public bool EstaActivo { get; set; }

    public LomoDeBurro(float altura, float longitud, float posicion)
    {
        Altura = altura;
        Longitud = longitud;
        Posicion = posicion;
        EstaActivo = true;
    }

    // M�todo para calcular el efecto del lomo de burro en el veh�culo
    public float CalcularEfecto(float velocidadVehiculo)
    {
        if (!EstaActivo)
            return 0f;

        // A mayor velocidad, mayor ser� el impacto del lomo de burro
        return velocidadVehiculo > 30 ? velocidadVehiculo * 0.2f : velocidadVehiculo * 0.1f;
    }
}

// Gestor de lomos de burro en la carretera
public class GestorLomosDeBurro
{
    private List<LomoDeBurro> _lomosDeBurro;

    public GestorLomosDeBurro()
    {
        _lomosDeBurro = new List<LomoDeBurro>();
    }

    public void AgregarLomoDeBurro(float altura, float longitud, float posicion)
    {
        _lomosDeBurro.Add(new LomoDeBurro(altura, longitud, posicion));
    }

    public LomoDeBurro ObtenerLomoDeBurroCercano(float posicionVehiculo, float margenDeteccion)
    {
        return _lomosDeBurro.FirstOrDefault(lomo =>
            Math.Abs(lomo.Posicion - posicionVehiculo) < margenDeteccion && lomo.EstaActivo);
    }
}

// Detector de lomos de burro para el veh�culo
public class DetectorLomoDeBurro
{
    private Vehiculo _vehiculo;
    private GestorLomosDeBurro _gestorLomos;
    private float _margenDeteccion = 5.0f;

    public event Action<float> LomoDeBurroDetectado;

    public DetectorLomoDeBurro(Vehiculo vehiculo, GestorLomosDeBurro gestorLomos)
    {
        _vehiculo = vehiculo;
        _gestorLomos = gestorLomos;
    }

    // Este m�todo se deber�a llamar en cada actualizaci�n del juego
    public void Actualizar()
    {
        var lomoCercano = _gestorLomos.ObtenerLomoDeBurroCercano(_vehiculo.Posicion, _margenDeteccion);

        if (lomoCercano != null)
        {
            float impacto = lomoCercano.CalcularEfecto(_vehiculo.VelocidadActual);
            LomoDeBurroDetectado?.Invoke(impacto);

            // Aplicar efecto al veh�culo directamente si es necesario
            AplicarEfectoAlVehiculo(impacto);
        }
    }

    private void AplicarEfectoAlVehiculo(float impacto)
    {
        // Aqu� podr�as reducir la velocidad del veh�culo
        _vehiculo.VelocidadActual = Math.Max(0, _vehiculo.VelocidadActual - impacto);

        // Tambi�n podr�as simular la sacudida del veh�culo
        if (impacto > 5.0f)
        {
            // Notificar al sistema de f�sica o al sistema de efectos visuales
            Console.WriteLine($"El veh�culo se sacude con intensidad {impacto}");
        }
    }
}