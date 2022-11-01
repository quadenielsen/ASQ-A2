using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace HiLoService
{
    [RunInstaller(true)]
    public partial class HiLoGameInstaller : System.Configuration.Install.Installer
    {
        public HiLoGameInstaller()
        {
            InitializeComponent();
        }

        private void HiLoServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void HiLoServiceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
