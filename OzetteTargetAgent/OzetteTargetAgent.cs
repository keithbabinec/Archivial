using OzetteLibrary.Logging;
using OzetteLibrary.ServiceCore;
using System.ServiceProcess;

namespace OzetteTargetAgent
{
    /// <summary>
    /// Contains service functionality.
    /// </summary>
    public partial class OzetteTargetAgent : ServiceBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public OzetteTargetAgent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Runs when the service start is triggered.
        /// </summary>
        /// <remarks>
        /// Long running initialization code can confuse the service control manager (thinks it may be a hang).
        /// Instead launch the initialization tasks in a seperate thread so control returns to the SCM immediately.
        /// </remarks>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            initHelper = new Initialization(new EventLogSetup());
            initHelper.Completed += InitHelper_Completed;
            initHelper.BeginStart(Properties.Settings.Default.Properties);
        }

        /// <summary>
        /// A reference to the initialization helper.
        /// </summary>
        private Initialization initHelper { get; set; }

        /// <summary>
        /// Callback event for when the initialization thread has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitHelper_Completed(object sender, System.EventArgs e)
        {
            if (initHelper.ResultCode == OzetteLibrary.Logging.StartupResults.Success)
            {
                // launch core
            }
            else
            {
                // safe exit without crash.
                // set the exit code so service control manager knows there is a problem.

                ExitCode = (int)initHelper.ResultCode;
                Stop();
            }
        }

        /// <summary>
        /// Runs when the service stop is triggered.
        /// </summary>
        protected override void OnStop()
        {
        }
    }
}
