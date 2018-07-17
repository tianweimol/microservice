namespace ID4.TokenService.Models
{
    public class AppSettingModel
    {
        /// <summary>
        /// 向这个IP对应的Consul注册
        /// </summary>
        public string ConsulIp { get; set; } = "127.0.0.1";

        /// <summary>
        /// 向Consul上的这个端口注册
        /// </summary>
        public int ConsulPort { get; set; } = 8500;

        public string Schema { get; set; } = "http://";

        public string ServiceName { get; set; }

        public string DataCenter { get; set; }
    }
}
