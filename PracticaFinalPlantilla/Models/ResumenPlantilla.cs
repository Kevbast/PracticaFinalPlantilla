namespace PracticaFinalPlantilla.Models
{
    public class ResumenPlantilla
    {
        public int Personas { get; set; }
        public int MaximoSalario { get; set; }
        public double MediaSalarial { get; set; }//SABEMOS QUE LA MEDIA SERÁ DECIMAL,LO PONEMOS DOUBLE

        public List<Plantilla> Empleadosp { get; set; }
    }
}
