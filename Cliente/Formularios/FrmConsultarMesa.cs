using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cliente.Modelo.ClienteTCP;
using Servidor;
using Cliente.Modelo.Clases; // Importar las clases necesarias para el manejo de localidades y mesas

namespace Cliente.Formularios
{
    public partial class FrmConsultarMesa : Form
    {
        private Localidad localidad;
        private ClienteTCP clienteTCP;
        private FrmCliente frmPadre;
        public FrmConsultarMesa()
        {
            InitializeComponent();
        }

        public FrmConsultarMesa(Localidad localidad, ClienteTCP cliente, FrmCliente padre)
        {
            InitializeComponent();
            this.localidad = localidad;
            this.clienteTCP = cliente;
            this.frmPadre = padre;
            numMesa.Maximum = localidad.CantidadMesas;
        }

        private async void btnConsultar_Click(object sender, EventArgs e)
        {
            int numeroMesa = Convert.ToInt32(numMesa.Value);
            
            string comando = $"ConsultarMesa|{numeroMesa},{localidad.Id}";

            // Usar el método centralizado del formulario padre para enviar el comando
            string respuesta = await frmPadre.EnviarComandoConManejoErrores(comando);

            if (respuesta == null)
            {
                // Error de comunicación ya manejado, solo salir
                return;
            }

            switch (respuesta)
            {
                case "1":
                    lblEstado.Text = "ABIERTA.";
                    await Votos();
                    break;
                case "0":
                    lblEstado.Text = "CERRADA.";
                    await Votos();
                    break;
                case "2":
                    lblEstado.Text = "NO EXISTE.";
                    dataGridView1.DataSource = null;
                    break;
                default:
                    MessageBox.Show("Error inesperado del servidor: " + respuesta);
                    break;
            }
        }

        private async Task Votos()
        {
            int numeroMesa = Convert.ToInt32(numMesa.Value);

            string comando = $"EnviarMesaResumen|{numeroMesa},{localidad.Id}";

            // Usar el método centralizado del formulario padre para enviar el comando
            string respuesta = await frmPadre.EnviarComandoConManejoErrores(comando);

            if (string.IsNullOrWhiteSpace(respuesta))
            {
                MessageBox.Show("No hay votos.");
                dataGridView1.DataSource = null;
                return;
            }

            // Procesar la respuesta esperada en formato: Nombre;Cantidad|Nombre;Cantidad|...
            var filas = respuesta.Trim().Split('|');
            var dt = new DataTable();
            dt.Columns.Add("Nombre", typeof(string));
            dt.Columns.Add("Cantidad", typeof(int));

            int totalVotos = 0;
            foreach (var fila in filas)
            {
                if (string.IsNullOrWhiteSpace(fila)) continue;
                var partes = fila.Split(';');
                if (partes.Length == 2 && int.TryParse(partes[1], out int cantidad))
                {
                    dt.Rows.Add(partes[0], cantidad);
                    totalVotos++;
                }
            }

            
                dataGridView1.DataSource = dt;
            
        }
    }
}
