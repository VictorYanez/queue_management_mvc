namespace queue_management.Enums
{
    public enum VisibilityStatus
    {
        /// Entidad inactiva #Inactive  (no visible en consultas normales)
        Inactivo = 0,

        /// Entidad activa #Active  y visible en el sistema
        Activo = 1,

        /// Entidad archivada #Archived  (solo para propósitos históricos)
        Archivado = 2,

        /// Acceso restringido #Restricted  (requiere permisos especiales)
        Restringido = 3
    }

}
