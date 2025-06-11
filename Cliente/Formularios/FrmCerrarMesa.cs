﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cliente.Modelo.Clases;    // Importar clases necesarias para el manejo de localidades y mesas
using Cliente.Modelo.ClienteTCP; // Importar la clase ClienteTCP para la comunicación con el servidor

namespace Servidor.Formularios
{
    public partial class FrmCerrarMesa : Form
    {
        // Variables para almacenar la localidad y el cliente TCP
        private Localidad localidad;
        private ClienteTCP clienteTCP;
        private FrmCliente frmPadre;

        public FrmCerrarMesa()
        {
            InitializeComponent();
        }
        // Constructor que recibe una localidad para inicializar el formulario
        public FrmCerrarMesa(Localidad localidad, ClienteTCP cliente, FrmCliente padre)
        {
            InitializeComponent();
            this.localidad = localidad;
            this.clienteTCP = cliente;
            this.frmPadre = padre;
            numMesa.Maximum = localidad.CantidadMesas;
        }
        /**
         * Método que se ejecuta al hacer clic en el botón de salida,
         * cerrando el formulario actual.
         */
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /**
         * Método asíncrono que se ejecuta al hacer clic en el botón para cerrar una mesa.
         * Envía un comando al servidor con el número de mesa y la localidad para solicitar el cierre.
         * Luego lee la respuesta del servidor y muestra un mensaje según el resultado:
         * - "1": mesa cerrada correctamente,
         * - "0": la mesa ya estaba cerrada,
         * - "2": la mesa no existe,
         * - cualquier otro valor: error inesperado.
         */
        private async void btnCerrarMesa_Click(object sender, EventArgs e)
        {
            int numeroMesa = Convert.ToInt32(numMesa.Value);

            string comando = $"CerrarMesa|{numeroMesa},{localidad.Id}";

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
                    MessageBox.Show("La mesa fue cerrada correctamente.");
                    break;
                case "0":
                    MessageBox.Show("La mesa ya estaba cerrada.");
                    break;
                case "2":
                    MessageBox.Show("La mesa no existe.");
                    break;
                default:
                    MessageBox.Show("Error inesperado del servidor: " + respuesta);
                    break;
            }
        }

    }
}
