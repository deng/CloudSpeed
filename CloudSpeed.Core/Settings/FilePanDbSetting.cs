namespace CloudSpeed.Settings
{
    public class FilePanDbSetting : IConnSetting
    {
        public string Connection { get; set; }

        public string MigrationsAssemblyName { get; set; }
        
        public DbServerType DbServer { get; set; }
    }
}
