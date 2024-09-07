using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProvisionKolayIK
{
    public partial class FrmReport : DevExpress.XtraEditors.XtraForm
    {
        public FrmReport(XtraReport xtraReport)
        {
            InitializeComponent();
            documentViewer1.DocumentSource= xtraReport;
        }
    }
}