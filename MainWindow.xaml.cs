using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ThirdProject
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Ponto> _torres { get; set; }
        public ChartValues<ObservablePoint> Torres { get; set; }
        public List<Ponto> _dispositivos { get; set; }
        public ChartValues<ObservablePoint> Dispositivos { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _dispositivos = LerDispositivosArquivoMobile();
            Dispositivos = new ChartValues<ObservablePoint>();
            foreach (Ponto d in _dispositivos)
                Dispositivos.Add(new ObservablePoint(d.X, d.Y));

            _torres = CriarTorres();
            Torres = new ChartValues<ObservablePoint>();
            foreach (Ponto t in _torres)
                Torres.Add(new ObservablePoint(t.X, t.Y));

            DataContext = this;

            AgruparDispositivosETorres();
        }

        public List<Ponto> CriarTorres()
        {
            var torres = new List<Ponto>();
            torres.Add(new Ponto(eTipoPonto.Torre, "Torre A", 0.1, 0.1));
            torres.Add(new Ponto(eTipoPonto.Torre, "Torre B", 0.3, 0.3));
            torres.Add(new Ponto(eTipoPonto.Torre, "Torre C", 0.7, 0.7));
            torres.Add(new Ponto(eTipoPonto.Torre, "Torre D", 0.9, 0.9));
            return torres;
        }

        public List<Ponto> LerDispositivosArquivoMobile()
        {
            var lista = new List<Ponto>();
            try
            {
                using (StreamReader sr = new StreamReader("mobile.txt"))
                {
                    int i = 0;
                    while (!sr.EndOfStream && i < 1000)
                    {
                        string[] linha = sr.ReadLine().Split(' ').Where(l => !string.IsNullOrEmpty(l)).ToArray();
                        lista.Add(new Ponto(eTipoPonto.Dispositivo, linha[0], Convert.ToDouble(linha[1], CultureInfo.InvariantCulture), Convert.ToDouble(linha[2], CultureInfo.InvariantCulture)));
                        i++;
                    }
                }
            }
            catch (IOException e)
            {

            }
            return lista;
        }

        private void AgruparDispositivosETorres()
        {
            foreach (Ponto torre in _torres)
                trTorres.Items.Add(new TreeViewItem() { Header = $"{torre.Nome} ( {torre.X.ToString().Replace(',', '.')}, {torre.Y.ToString().Replace(',', '.')})", Tag = torre.Nome });

            foreach (Ponto dispositivo in _dispositivos)
            {
                Ponto torreMaisProxima = null;
                double distancia = 0;
                foreach (Ponto torre in _torres)
                {
                    if (distancia == 0 && torreMaisProxima == null)
                    {
                        distancia = DistanciaDeManhattan(torre, dispositivo);
                        torreMaisProxima = torre;
                    }
                    if (DistanciaDeManhattan(torre, dispositivo) < distancia)
                        torreMaisProxima = torre;
                }
                foreach (TreeViewItem item in trTorres.Items)
                    if (item.Tag.ToString() == torreMaisProxima.Nome)
                        item.Items.Add(new TreeViewItem() { Header = $"{dispositivo.Nome} ({dispositivo.X.ToString().Replace(',', '.')}, {dispositivo.Y.ToString().Replace(',', '.')})", Tag = dispositivo.Nome });
            }
        }

        public double DistanciaDeManhattan(Ponto torre, Ponto dispotivo)
        {
            return Math.Abs((torre.X - dispotivo.X) + (torre.Y - dispotivo.Y));
        }
    }

    public enum eTipoPonto { Dispositivo, Torre }
    public class Ponto
    {
        public eTipoPonto eTipoPonto { get; set; }
        public string Nome { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public Ponto(eTipoPonto tipoPonto, string nome, double x, double y)
        {
            eTipoPonto = tipoPonto;
            Nome = nome;
            X = x;
            Y = y;
        }
    }
}
