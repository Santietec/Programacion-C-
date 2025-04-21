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

    // Método para calcular el efecto del lomo de burro en el vehículo
    public float CalcularEfecto(float velocidadVehiculo)
    {
        if (!EstaActivo)
            return 0f;

        // A mayor velocidad, mayor será el impacto del lomo de burro
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

// Detector de lomos de burro para el vehículo
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

    // Este método se debería llamar en cada actualización del juego
    public void Actualizar()
    {
        var lomoCercano = _gestorLomos.ObtenerLomoDeBurroCercano(_vehiculo.Posicion, _margenDeteccion);

        if (lomoCercano != null)
        {
            float impacto = lomoCercano.CalcularEfecto(_vehiculo.VelocidadActual);
            LomoDeBurroDetectado?.Invoke(impacto);

            // Aplicar efecto al vehículo directamente si es necesario
            AplicarEfectoAlVehiculo(impacto);
        }
    }

    private void AplicarEfectoAlVehiculo(float impacto)
    {
        // Aquí podrías reducir la velocidad del vehículo
        _vehiculo.VelocidadActual = Math.Max(0, _vehiculo.VelocidadActual - impacto);

        // También podrías simular la sacudida del vehículo
        if (impacto > 5.0f)
        {
            // Notificar al sistema de física o al sistema de efectos visuales
            Console.WriteLine($"El vehículo se sacude con intensidad {impacto}");
        }
    }
}