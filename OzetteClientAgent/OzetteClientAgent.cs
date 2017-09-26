using System.ServiceProcess;

namespace OzetteClientAgent
{
    public partial class OzetteClientAgent : ServiceBase
    {
        public OzetteClientAgent()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
