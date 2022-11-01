
namespace HiLoService
{
    partial class HiLoGameInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.HiLoServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.HiLoServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // HiLoServiceProcessInstaller
            // 
            this.HiLoServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.HiLoServiceProcessInstaller.Password = null;
            this.HiLoServiceProcessInstaller.Username = null;
            this.HiLoServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.HiLoServiceProcessInstaller_AfterInstall);
            // 
            // HiLoServiceInstaller
            // 
            this.HiLoServiceInstaller.ServiceName = "HiLoGameService";
            this.HiLoServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.HiLoServiceInstaller_AfterInstall);
            // 
            // HiLoGameInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.HiLoServiceProcessInstaller,
            this.HiLoServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller HiLoServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller HiLoServiceInstaller;
    }
}