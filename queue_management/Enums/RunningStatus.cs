namespace queue_management.Enums
{
    public enum RunningStatus
    {
        // Ventanilla cerrada #Closed permanentemente (no visible en consultas normales)
        Cerrado = 0,
        // Lista para usarse #Opened
        Abierto = 1,
        // En mantenimiento #Maintenance
        Mantenimiento = 2,
        // Fuera de servicio temporal 
        OutOfService = 3
    }
}
