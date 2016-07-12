﻿namespace DfBAdminToolkit.View {

    using Common.Utils;
    using Common.Services;
    using Model;
    using Presenter;
    using Services;
    using System;
    using System.Net;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Forms;
    using Newtonsoft.Json;

    public partial class MainView : Form, IMainView {
        public event EventHandler CommandQuitApplication;
        public event EventHandler DataChanged;

        #region Runtime components

        private CustomTabControl _tabControl;
        private IList<TabPage> _tabPages;

        private new enum TabIndex : int {
            TextSearch = 0,
            DumpContent = 1,
            Provisioning = 2,
            Groups = 3,
            Devices = 4,
            DataMigration = 5
        }

        #endregion Runtime components

        public bool ComponentEventsWired {
            get; set;
        }

        public SynchronizationContext SyncContext { get; set; }

        public MainView() {
            InitializeComponent();
            InitializeRuntimeComponent();
            Initialize();
            WireComponentEvents();
        }

        ~MainView() {
            UnWireComponentEvents();
        }

        public void WireComponentEvents() {
            if (!ComponentEventsWired) {
                // TODO: event wiring
                _tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
                settingsToolStripMenuItem.Click += SettingsToolStripMenuItem_Click;
                exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
                ComponentEventsWired = true;
            }
        }

        public void UnWireComponentEvents() {
            if (ComponentEventsWired) {
                // TODO: event release
                _tabControl.SelectedIndexChanged -= TabControl_SelectedIndexChanged;
                settingsToolStripMenuItem.Click -= SettingsToolStripMenuItem_Click;
                exitToolStripMenuItem.Click -= ExitToolStripMenuItem_Click;
                ComponentEventsWired = false;
            }
        }

        public void Initialize() {
            SyncContext = SynchronizationContext.Current;
            StartPosition = FormStartPosition.CenterScreen;
            ComponentEventsWired = false;
            RegisterTabPages();
            //get build version and update title bar for basic version
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = "Dropbox Business Admin Toolkit v" + version;
            //token not configured yet
            bool checkToken = FileUtil.TokenCheck();
            if (!checkToken) {
                ShowErrorMessage(ErrorMessages.MISSING_TOKEN, ErrorMessages.DLG_DEFAULT_TITLE);
            }
            //if token configured get team stats and put in title text
            if (checkToken)
            {
                this.UpdateTitleBarTeamStats();
            }
            this.CheckLatestVersion();
        }

        public void ShowView() {
            this.Show();
        }

        public void HideView() {
            this.Hide();
        }

        private void InitializeRuntimeComponent() {
            _tabControl = new CustomTabControl();
            _tabControl.Name = "tabControl_Main";
            _tabControl.Dock = DockStyle.Fill;
            _tabControl.DisplayStyle = TabStyle.Angled;
            _tabControl.RightToLeftLayout = false;
            _tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            _tabControl.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel_MainGroup.Controls.Add(_tabControl);

            // add pages.
            _tabPages = new List<TabPage>();
            _tabPages.Add(new TabPage() {
                Name = "tabPage_FullTextSearch",
                Text = "Full Text Search",
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 3, 5, 3),
                UseVisualStyleBackColor = true
            });
            _tabPages.Add(new TabPage() {
                Name = "tabPage_DumpUserContents",
                Text = "Dump User Contents",
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 3, 5, 3),
                UseVisualStyleBackColor = true
            });
            _tabPages.Add(new TabPage() {
                Name = "tabPage_Provisioning",
                Text = "Users",
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 3, 5, 3),
                UseVisualStyleBackColor = true
            });
            _tabPages.Add(new TabPage()
            {
                Name = "tabPage_Groups",
                Text = "Groups",
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 3, 5, 3),
                UseVisualStyleBackColor = true
            });
            _tabPages.Add(new TabPage() {
                Name = "tabPage_Devices",
                Text = "Devices",
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 3, 5, 3),
                UseVisualStyleBackColor = true
            });
            _tabPages.Add(new TabPage() {
                Name = "tabPage_DataMigration",
                Text = "Data Migration",
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 3, 3, 5),
                UseVisualStyleBackColor = true
            });
        }

        #region Events

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            RepaintTabControlBackground();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e) {
            RepaintTabControlBackground();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e) {
            ISettingsPresenter presenter = GetSettingPresenter();
            presenter.ShowSettings(this);
        }

        public void UpdateTitleBarTeamStats()
        {
            int licensed = 0;
            int provisioned = 0;
            string name = string.Empty;

            //do a call to get team info
            IMemberServices service = service = new MemberServices(ApplicationResource.BaseUrl, ApplicationResource.ApiVersion);
            service.GetInfoUrl = ApplicationResource.ActionGetInfo;
            service.UserAgentVersion = ApplicationResource.UserAgent;
            string AccessToken = ApplicationResource.DefaultAccessToken;

            IDataResponse response = service.GetInfo(AccessToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Data != null)
                {
                    string data = response.Data.ToString();
                    dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(data);

                    name = jsonData["name"];
                    licensed = jsonData["num_licensed_users"];
                    provisioned = jsonData["num_provisioned_users"];
                }
            }
            //get toolkit version and build title text
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = "Dropbox Business Admin Toolkit v" + version + "            Team: " + name + "       Licensed Users: " + licensed.ToString() + "       Provisioned Users: " + provisioned.ToString();
            this.Update();
            this.Refresh();
        }

        public void CheckLatestVersion()
        {
            GitHubService service = new GitHubService();
            Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Version latestVersion = new Version(service.LatestVersion());
            if (latestVersion > currentVersion)
            {
                ShowErrorMessage(@"There is a newer version of the Admin Toolkit available.", @"New Version Available");
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion Events

        #region Slots & helpers

        public void EnableView(bool enable) {
            this.tableLayoutPanel_MainGroup.Enabled = enable;
            this.tableLayoutPanel_MainGroup.Update();
        }

        public void EnableLoadingSpinner(bool enable) {
            this.loadingCircleToolStripMenuItem_Main.Visible = enable;
            this.loadingCircleToolStripMenuItem_Main.LoadingCircleControl.Active = enable;
            this.loadingCircleToolStripMenuItem_Main.LoadingCircleControl.Update();
            this.statusStrip_Main.Update();
        }

        public void UpdateProgressText(string text) {
            this.toolStripStatusLabel_Progress.Text = text;
            this.statusStrip_Main.Update();
        }

        private void RepaintTabControlBackground() {
            // enforce unified tab color
            _tabControl.SelectedTab.BackColor = System.Drawing.SystemColors.Control;
        }

        #endregion Slots & helpers

        #region Child view registration

        private void RegisterTabPages() {
            // register full text search view
            _tabPages[(int)TabIndex.TextSearch].Controls.Add(CreateTextSearchView());
            _tabPages[(int)TabIndex.DumpContent].Controls.Add(CreateDumpUserContentView());
            _tabPages[(int)TabIndex.Provisioning].Controls.Add(CreateProvisionView());
            _tabPages[(int)TabIndex.Groups].Controls.Add(CreateGroupsView());
            _tabPages[(int)TabIndex.Devices].Controls.Add(CreateDevicesView());
            _tabPages[(int)TabIndex.DataMigration].Controls.Add(CreateDataMigrationView());

            _tabControl.Controls.Add(_tabPages[(int)TabIndex.TextSearch]);
            _tabControl.Controls.Add(_tabPages[(int)TabIndex.DumpContent]);
            _tabControl.Controls.Add(_tabPages[(int)TabIndex.Provisioning]);
            _tabControl.Controls.Add(_tabPages[(int)TabIndex.Groups]);
            _tabControl.Controls.Add(_tabPages[(int)TabIndex.Devices]);
            _tabControl.Controls.Add(_tabPages[(int)TabIndex.DataMigration]);

            // select first tab
            _tabControl.SelectTab((int)TabIndex.TextSearch);
        }

        private TextSearchView CreateTextSearchView() {
            ITextSearchModel model = new TextSearchModel();
            ITextSearchView view = new TextSearchView();
            ITextSearchPresenter presenter = SimpleResolver.Instance.Get<ITextSearchPresenter>(
                new object[] { model, view }
            );
            view.ShowView();
            return view as TextSearchView;
        }

        private DumpUserContentView CreateDumpUserContentView() {
            IDumpUserContentModel model = new DumpUserContentModel();
            IDumpUserContentView view = new DumpUserContentView();
            IDumpUserContentPresenter presenter = SimpleResolver.Instance.Get<IDumpUserContentPresenter>(
                new object[] { model, view }
            );
            view.ShowView();
            return view as DumpUserContentView;
        }

        private ProvisioningView CreateProvisionView() {
            IProvisioningModel model = new ProvisioningModel();
            IProvisioningView view = new ProvisioningView();
            IProvisioningPresenter presenter = SimpleResolver.Instance.Get<IProvisioningPresenter>(
                new object[] { model, view }
            );
            view.ShowView();
            return view as ProvisioningView;
        }

        private GroupsView CreateGroupsView()
        {
            IGroupsModel model = new GroupsModel();
            IGroupsView view = new GroupsView();
            IGroupsPresenter presenter = SimpleResolver.Instance.Get<IGroupsPresenter>(
                new object[] { model, view }
            );
            view.ShowView();
            return view as GroupsView;
        }

        private DevicesView CreateDevicesView() {
            IDevicesModel model = new DevicesModel();
            IDevicesView view = new DevicesView();
            IDevicesPresenter presenter = SimpleResolver.Instance.Get<IDevicesPresenter>(
                new object[] { model, view }
            );
            view.ShowView();
            return view as DevicesView;
        }

        private DataMigrationView CreateDataMigrationView() {
            IDataMigrationModel model = new DataMigrationModel();
            IDataMigrationView view = new DataMigrationView();
            IDataMigrationPresenter presenter = SimpleResolver.Instance.Get<IDataMigrationPresenter>(
                new object[] { model, view }
            );
            view.ShowView();
            return view as DataMigrationView;
        }

        private ISettingsPresenter GetSettingPresenter() {
            ISettingsModel model = new SettingsModel();
            ISettingsView view = new SettingsView();
            ISettingsPresenter presenter = SimpleResolver.Instance.Get<ISettingsPresenter>(
                new object[] { model, view }
            );
            return presenter;
        }

        #endregion Child view registration

        #region Message box

        public void ShowErrorMessage(string message, string title) {
            MessageBoxUtil.ShowError(this, message, title);
        }

        public void ShowInfoMessage(string message) {
            MessageBoxUtil.ShowInfo(this, message);
        }

        #endregion Message box

    }
}