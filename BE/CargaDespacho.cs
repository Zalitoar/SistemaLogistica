namespace BE
{
    // Proyección de lectura/edición de carga; no es una segunda entidad de mercadería.
    public class CargaDespacho
    {
        public int IdMercaderia { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal CantidadPrevista { get; set; }
        public decimal CantidadPreparada { get; set; }
        public decimal Diferencia { get { return CantidadPreparada - CantidadPrevista; } }
    }
}
