import os
import sys
import re
import yaml
from typing import Dict, List, Tuple



template_gitignore = r"""
.vs/
app/bin/
app/obj/
bridge/bin/
bridge/obj/
module/bin/
module/obj/
winform/bin/
winform/obj/
*.user
"""

template_sln = r"""
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 16
VisualStudioVersion = 16.0.31112.23
MinimumVisualStudioVersion = 10.0.40219.1
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "app", "app\app.csproj", "{F2F84FF1-7987-476F-8F03-8316DB67217F}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "module", "module\module.csproj", "{124ACB03-D1AC-479D-B95A-DE9F13C266FA}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "winform", "winform\winform.csproj", "{ECEBC9A0-5079-470A-A380-5B80756DEA61}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "bridge", "bridge\bridge.csproj", "{8DF06770-ADA4-407D-ABAF-6C222C73962E}"
EndProject
Global
        GlobalSection(SolutionConfigurationPlatforms) = preSolution
                Debug|Any CPU = Debug|Any CPU
                Release|Any CPU = Release|Any CPU
        EndGlobalSection
        GlobalSection(ProjectConfigurationPlatforms) = postSolution
                {124ACB03-D1AC-479D-B95A-DE9F13C266FA}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                {124ACB03-D1AC-479D-B95A-DE9F13C266FA}.Debug|Any CPU.Build.0 = Debug|Any CPU
                {124ACB03-D1AC-479D-B95A-DE9F13C266FA}.Release|Any CPU.ActiveCfg = Release|Any CPU
                {124ACB03-D1AC-479D-B95A-DE9F13C266FA}.Release|Any CPU.Build.0 = Release|Any CPU
                {F2F84FF1-7987-476F-8F03-8316DB67217F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                {F2F84FF1-7987-476F-8F03-8316DB67217F}.Debug|Any CPU.Build.0 = Debug|Any CPU
                {F2F84FF1-7987-476F-8F03-8316DB67217F}.Release|Any CPU.ActiveCfg = Release|Any CPU
                {F2F84FF1-7987-476F-8F03-8316DB67217F}.Release|Any CPU.Build.0 = Release|Any CPU
                {ECEBC9A0-5079-470A-A380-5B80756DEA61}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                {ECEBC9A0-5079-470A-A380-5B80756DEA61}.Debug|Any CPU.Build.0 = Debug|Any CPU
                {ECEBC9A0-5079-470A-A380-5B80756DEA61}.Release|Any CPU.ActiveCfg = Release|Any CPU
                {ECEBC9A0-5079-470A-A380-5B80756DEA61}.Release|Any CPU.Build.0 = Release|Any CPU
                {8DF06770-ADA4-407D-ABAF-6C222C73962E}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                {8DF06770-ADA4-407D-ABAF-6C222C73962E}.Debug|Any CPU.Build.0 = Debug|Any CPU
                {8DF06770-ADA4-407D-ABAF-6C222C73962E}.Release|Any CPU.ActiveCfg = Release|Any CPU
                {8DF06770-ADA4-407D-ABAF-6C222C73962E}.Release|Any CPU.Build.0 = Release|Any CPU
        EndGlobalSection
        GlobalSection(SolutionProperties) = preSolution
                HideSolutionNode = FALSE
        EndGlobalSection
        GlobalSection(ExtensibilityGlobals) = postSolution
                SolutionGuid = {089BAFA9-97A5-468A-9FA8-D368A7EE49A6}
        EndGlobalSection
EndGlobal

"""

template_proj_app = r"""
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\module\module.csproj" />
    <ProjectReference Include="..\winform\winform.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="oelMVCS" Version="1.1.0" />
  </ItemGroup>

</Project>
"""

template_proj_bridge = r"""
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <AssemblyName>{{org}}-{{mod}}-bridge</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="oelMVCS" Version="1.1.0" />
  </ItemGroup>

</Project>
"""

template_proj_module = r"""
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <AssemblyName>{{org}}-{{mod}}-module</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\bridge\bridge.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="oelMVCS" Version="1.1.0" />
  </ItemGroup>

</Project>
"""

template_proj_winform = r"""
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <AssemblyName>{{org}}-{{mod}}-winform</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\bridge\bridge.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="oelMVCS" Version="1.1.0" />
  </ItemGroup>

</Project>
"""

template_app_AppFacade_cs = r"""
using XTC.oelMVCS;

namespace app
{
    class AppFacade : View.Facade
    {
        public const string NAME = "AppFacade";
        public RootForm rootForm { get; set; }
    }//class
}//namespace
"""

template_app_AppView_cs = r"""
using System.Collections.Generic;
using XTC.oelMVCS;

namespace app
{
    class AppView : View
    {
        public const string NAME = "AppView";

        private AppFacade appFacade = null;

        protected override void preSetup()
        {
            appFacade = findFacade(AppFacade.NAME) as AppFacade;
        }

        protected override void setup()
        {
            route("/module/view/attach", this.handleAttachView);
        }

        private void handleAttachView(Model.Status _status, object _data)
        {
            getLogger().Trace("attach view");
            Dictionary<string, object> data = _data as Dictionary<string, object>;
            foreach(string path in data.Keys)
            {
                appFacade.rootForm.AddPath(path, data[path]);
            }
        }
    }//class
}//namespace
"""

template_app_AppConfig_cs = r"""
using System.Text.Json;
using XTC.oelMVCS;

namespace app
{
    class ConfigSchema
    {
        public string domain {get;set;}
    }
    class AppConfig: Config
    {
        public override void Merge(string _content)
        {
            ConfigSchema schema = JsonSerializer.Deserialize<ConfigSchema>(_content);
            fields_["domain"] = Any.FromString(schema.domain);
        }
    }//class
}//namespace
"""

template_app_ConsoleLogger_cs = r"""



using System;
using System.Drawing;
using XTC.oelMVCS;

namespace app
{
    class ConsoleLogger : Logger
    {
        public System.Windows.Forms.RichTextBox rtbLog { get; set; }
        protected override void trace(string _categoray, string _message)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("TRACE | {0} > {1}", _categoray, _message), Color.Gray);
        }

        protected override void debug(string _categoray, string _message)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("DEBUG | {0} > {1}", _categoray, _message), Color.Blue);
        }

        protected override void info(string _categoray, string _message)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("INFO | {0} > {1}", _categoray, _message), Color.Green);
        }

        protected override void warning(string _categoray, string _message)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("WARN | {0} > {1}", _categoray, _message), Color.Orange);
        }

        protected override void error(string _categoray, string _message)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("ERROR | {0} > {1}", _categoray, _message), Color.Red);
        }

        protected override void exception(Exception _exception)
        {
            if (rtbLog.IsDisposed)
                return;
            this.appendTextColorful(string.Format("EXCEPT | > {0}", _exception.ToString()), Color.Purple);
        }

        private void appendTextColorful(string addtext, Color color)
        {
            addtext += Environment.NewLine;
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText(addtext);
        }
    }//class
}//namespace
"""

template_app_Program_cs = r"""

using System;
using System.Windows.Forms;
using {{org}}.Module.{{mod}};
using XTC.oelMVCS;

namespace app
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RootForm rootForm = new RootForm();

            ConsoleLogger logger = new ConsoleLogger();
            logger.rtbLog = rootForm.getLoggerUi();
            Config config = new AppConfig();
            ConfigSchema schema = new ConfigSchema();
            schema.domain = rootForm.getDomainUi().Text;
            config.Merge(System.Text.Json.JsonSerializer.Serialize(schema));
            Framework framework = new Framework();
            framework.setLogger(logger);
            framework.setConfig(config);
            framework.Initialize();

            AppFacade appFacade = new AppFacade();
            appFacade.rootForm = rootForm;
            framework.getStaticPipe().RegisterFacade(AppFacade.NAME, appFacade);
            AppView appView = new AppView();
            framework.getStaticPipe().RegisterView(AppView.NAME, appView);

            // 注册模块窗体
            FormRoot formRoot = new FormRoot();
            formRoot.Inject(framework);
            formRoot.Register();
            // 注册模块逻辑
            ModuleRoot moduleRoot = new ModuleRoot();
            moduleRoot.Inject(framework);
            moduleRoot.Register();

            framework.Setup();

            Application.Run(rootForm);

            moduleRoot.Cancel();
            formRoot.Cancel();

            framework.Dismantle();
            framework.Release();
        }
    }
}
"""

template_app_RootForm_cs = r"""
using System.Collections.Generic;
using System.Windows.Forms;

namespace app
{
    public class RootForm: Form
    {
        public RootForm()
        {
            InitializeComponent();
        }

        public RichTextBox getLoggerUi()
        {
            return this.rtbLog;
        }

        public TextBox getDomainUi()
        {
            return this.tbDomain;
        }

        private TabControl tcPages;
        private TreeView tvPages;
        private Dictionary<string, TabPage> pages = new Dictionary<string, TabPage>();
        private RichTextBox rtbLog;
        private TextBox tbDomain;

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void AddPath(string _path, object _page)
        {
            ContainerControl page = _page as ContainerControl;
            //this.Controls.Add(panel);
            string[] sections = _path.Split("/");
            var nodes = this.tvPages.Nodes;
            foreach(string section in sections)
            {
                if(string.IsNullOrEmpty(section))
                    continue;
                var found = nodes.Find(section, false);
                if(found.Length == 0)
                {
                    TreeNode newNode = new TreeNode();
                    newNode.Name = section;
                    newNode.Text = section;
                    nodes.Add(newNode);
                    found = new TreeNode[]{newNode};
                }
                nodes = found[0].Nodes;
            }
            TabPage tabPage = new TabPage();
            tabPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            tabPage.Location = new System.Drawing.Point(10, 10);
            tabPage.Name = _path;
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(760, 660);
            tabPage.TabIndex = 0;
            tabPage.Text = _path;
            tabPage.UseVisualStyleBackColor = true;
            tabPage.Controls.Add(page);
            this.tcPages.Controls.Add(tabPage);
            this.pages[_path] = tabPage;
            this.tvPages.ExpandAll();
        }

        private void InitializeComponent()
        {
            this.tcPages = new System.Windows.Forms.TabControl();
            this.tvPages = new System.Windows.Forms.TreeView();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.tbDomain = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tcPages
            // 
            this.tcPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcPages.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tcPages.ItemSize = new System.Drawing.Size(0, 1);
            this.tcPages.Location = new System.Drawing.Point(216, 61);
            this.tcPages.Name = "tcPages";
            this.tcPages.SelectedIndex = 0;
            this.tcPages.Size = new System.Drawing.Size(780, 468);
            this.tcPages.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tcPages.TabIndex = 1;
            // 
            // tvPages
            // 
            this.tvPages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvPages.Location = new System.Drawing.Point(13, 20);
            this.tvPages.Name = "tvPages";
            this.tvPages.PathSeparator = "/";
            this.tvPages.Size = new System.Drawing.Size(188, 509);
            this.tvPages.TabIndex = 2;
            this.tvPages.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvPages_AfterSelect);
            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLog.Location = new System.Drawing.Point(13, 547);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(983, 170);
            this.rtbLog.TabIndex = 3;
            this.rtbLog.Text = "";
            // 
            // tbDomain
            // 
            this.tbDomain.Location = new System.Drawing.Point(231, 20);
            this.tbDomain.Name = "tbDomain";
            this.tbDomain.Size = new System.Drawing.Size(416, 23);
            this.tbDomain.TabIndex = 4;
            this.tbDomain.Text = "http://localhost:8080";
            // 
            // RootForm
            // 
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.tbDomain);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.tvPages);
            this.Controls.Add(this.tcPages);
            this.Name = "RootForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void tvPages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node.Nodes.Count > 0)
                return;
            string fullpath = "/" + node.FullPath;
            TabPage page;
            if (!this.pages.TryGetValue(fullpath, out page))
                return;
            this.tcPages.SelectedTab = page;
        }
    }

}
"""

template_app_RootForm_resx = r"""
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
</root>
"""

templete_bridge_view_cs = r"""
using XTC.oelMVCS;
namespace {{org}}.Module.{{mod}}
{
    public interface I{{service}}ViewBridge : View.Facade.Bridge
    {
{{rpc}}
    }
}
"""

templete_bridge_ui_cs = r"""
using XTC.oelMVCS;
namespace {{org}}.Module.{{mod}}
{
    public interface I{{service}}UiBridge : View.Facade.Bridge
    {
        object getRootPanel();
        void Alert(string _message);
    }
}
"""

template_module_ModuleRoot_cs = r"""
using System.Collections.Generic;
using XTC.oelMVCS;

namespace {{org}}.Module.{{mod}}
{
    public class ModuleRoot
    {
        public ModuleRoot()
        {
        }

        public void Inject(Framework _framework)
        {
            framework_ = _framework;
        }

        public void Register()
        {
{{register}}
        }

        public void Cancel()
        {
{{cancel}}
        }

        private Framework framework_ = null;
    }
}
"""

template_module_Json_Convert_cs = r"""

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using XTC.oelMVCS;

namespace {{org}}.Module.{{mod}}
{
    /// <summary>
    /// 用于将请求数据序列化为json
    /// </summary>
    class AnyConverter : JsonConverter<Any>
    {
        public override Any Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Any _value, JsonSerializerOptions options)
        {
            if(_value.IsString())
                writer.WriteStringValue(_value.AsString());
            else if (_value.IsInt())
                writer.WriteNumberValue(_value.AsInt());
            else if (_value.IsLong())
                writer.WriteNumberValue(_value.AsLong());
            else if (_value.IsFloat())
                writer.WriteNumberValue(_value.AsFloat());
            else if (_value.IsDouble())
                writer.WriteNumberValue(_value.AsDouble());
            else if (_value.IsBool())
                writer.WriteBooleanValue(_value.AsBool());
            else if(_value.IsStringAry())
            {
                writer.WriteStartArray();
                foreach(string v in _value.AsStringAry())
                {
                    writer.WriteStringValue(v);
                }
                writer.WriteEndArray();
            }
            else if (_value.IsInt32Ary())
            {
                writer.WriteStartArray();
                foreach (int v in _value.AsInt32Ary())
                {
                    writer.WriteNumberValue(v);
                }
                writer.WriteEndArray();
            }
            else if (_value.IsInt64Ary())
            {
                writer.WriteStartArray();
                foreach (long v in _value.AsInt64Ary())
                {
                    writer.WriteNumberValue(v);
                }
                writer.WriteEndArray();
            }
            else if (_value.IsFloat32Ary())
            {
                writer.WriteStartArray();
                foreach (float v in _value.AsFloat32Ary())
                {
                    writer.WriteNumberValue(v);
                }
                writer.WriteEndArray();
            }
            else if (_value.IsFloat64Ary())
            {
                writer.WriteStartArray();
                foreach (double v in _value.AsFloat64Ary())
                {
                    writer.WriteNumberValue(v);
                }
                writer.WriteEndArray();
            }
            else if (_value.IsBoolAry())
            {
                writer.WriteStartArray();
                foreach (bool v in _value.AsBoolAry())
                {
                    writer.WriteBooleanValue(v);
                }
                writer.WriteEndArray();
            }
        }
    }//class

    /// <summary>
    /// 用于将json反序列化为回复数据
    /// </summary>
    class FieldConverter : JsonConverter<Proto.Field>
    {
        public override Proto.Field Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                List<string> ary = new List<string>();
                while (reader.Read())
                {
                   if(reader.TokenType == JsonTokenType.EndArray)
                    {
                        break;
                    }
                    string s = reader.GetString();
                    ary.Add(s);
                }
                return Proto.Field.FromStringAry(ary.ToArray());
            }
            return Proto.Field.FromString(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Proto.Field _value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }//class
}//namespace
"""

template_module_Model_cs = r"""
using System;
using XTC.oelMVCS;

namespace {{org}}.Module.{{mod}}
{
    public class {{service}}Model : Model
    {
        public const string NAME = "{{mod}}.{{service}}Model";

        public class {{service}}Status : Model.Status
        {
            public const string NAME = "{{mod}}.{{service}}Status";
        }

        protected override void preSetup()
        {
            Error err;
            status_ = spawnStatus<{{service}}Status>({{service}}Status.NAME, out err);
        }

        protected override void setup()
        {
            getLogger().Trace("setup {{service}}Model");
        }

        protected override void preDismantle()
        {
            Error err;
            killStatus({{service}}Status.NAME, out err);
        }

        private {{service}}Status status
        {
            get
            {
                return status_ as {{service}}Status;
            }
        }
    }
}
"""

template_module_View_cs = r"""
using System;
using System.Collections.Generic;
using XTC.oelMVCS;

namespace {{org}}.Module.{{mod}}
{
    public class {{service}}View: View
    {
        public const string NAME = "{{mod}}.{{service}}View";

        private Facade facade = null;
        private {{service}}Model model = null;
        private I{{service}}UiBridge bridge = null;

        protected override void preSetup()
        {
            model = findModel({{service}}Model.NAME) as {{service}}Model;
            var service = findService({{service}}Service.NAME) as {{service}}Service;
            facade = findFacade("{{mod}}.{{service}}Facade");
            {{service}}ViewBridge vb = new {{service}}ViewBridge();
            vb.view = this;
            vb.service = service;
            facade.setViewBridge(vb);
        }

        protected override void setup()
        {
            getLogger().Trace("setup {{service}}View");
{{routers}}
        }

        protected override void postSetup()
        {
            bridge = facade.getUiBridge() as I{{service}}UiBridge;
            object rootPanel = bridge.getRootPanel();
            // 通知主程序挂载界面
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["/{{org}}/{{mod}}/{{service}}"] = rootPanel;
            model.Broadcast("/module/view/attach", data);
        }
{{handlers}}
    }
}
"""

template_module_Controller_cs = r"""
using System;
using XTC.oelMVCS;

namespace {{org}}.Module.{{mod}}
{
    public class {{service}}Controller: Controller
    {
        public const string NAME = "{{mod}}.{{service}}Controller";

        protected override void setup()
        {
            getLogger().Trace("setup {{service}}Controller");
        }
    }
}
"""

template_module_ViewBridge_cs = r"""
using XTC.oelMVCS;
namespace {{org}}.Module.{{mod}}
{
    public class {{service}}ViewBridge : I{{service}}ViewBridge
    {
        public {{service}}View view{ get; set; }
        public {{service}}Service service{ get; set; }

{{rpc}}

    }
}
"""

template_module_Service_cs = r"""
using System.IO;
using System.Net;
using System.Text.Json;
using System.Collections.Generic;
using XTC.oelMVCS;

namespace {{org}}.Module.{{mod}}
{
    public class {{service}}Service: Service
    {
        public const string NAME = "{{mod}}.{{service}}Service";
        private {{service}}Model model = null;

        protected override void preSetup()
        {
            model = findModel({{service}}Model.NAME) as {{service}}Model;
        }

        protected override void setup()
        {
            getLogger().Trace("setup {{service}}Service");
        }
{{rpc}}

        protected override void asyncRequest(string _url, string _method, Dictionary<string, Any> _params, OnReplyCallback _onReply, OnErrorCallback _onError, Options _options)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_url); 
            req.Method = _method;
            req.ContentType =
            "application/json;charset=utf-8";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new AnyConverter());
            string json = System.Text.Json.JsonSerializer.Serialize(_params, options);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
            }
            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            if(rsp == null)
            {
                _onError(Error.NewNullErr("HttpWebResponse is null"));
                return;
            }
            if(rsp.StatusCode != HttpStatusCode.OK)
            {
                rsp.Close();
                _onError(new Error(rsp.StatusCode.GetHashCode(), "HttpStatusCode != 200"));
                return;
            }
            string reply = "";
            StreamReader sr;
            using (sr = new StreamReader(rsp.GetResponseStream()))
            {
                reply = sr.ReadToEnd();
            }
            sr.Close();
            _onReply(reply);
        }
    }
}
"""


template_module_Protocol_cs = r"""
using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using XTC.oelMVCS;

namespace {{org}}.Module.{{mod}}.Proto
{
public class Field
    {
        public enum Tag
        {
            NULL = 0,
            StringValue = 1,
            IntValue = 2,
            LongValue = 3,
            FloatValue = 4,
            DoubleValue = 5,
            BoolValue = 6,
            StringAryValue = 11,
            IntAryValue = 12,
            LongAryValue = 13,
            FloatAryValue = 14,
            DoubleAryValue = 15,
            BoolAryValue = 16
        }

        private string value_ = "";
        private Tag tag_ = Tag.NULL;

        public Field()
        {
        }

        public static Field FromString(string _value)
        {
            Field any = new Field();
            any.tag_ = Tag.StringValue;
            any.value_ = _value;
            return any;
        }

        public static Field FromFloat(float _value)
        {
            Field any = new Field();
            any.tag_ = Tag.FloatValue;
            any.value_ = _value.ToString();
            return any;
        }

        public static Field FromDouble(double _value)
        {
            Field any = new Field();
            any.tag_ = Tag.DoubleValue;
            any.value_ = _value.ToString();
            return any;
        }

        public static Field FromBool(bool _value)
        {
            Field any = new Field();
            any.tag_ = Tag.BoolValue;
            any.value_ = _value.ToString();
            return any;
        }

        public static Field FromInt(int _value)
        {
            Field any = new Field();
            any.tag_ = Tag.IntValue;
            any.value_ = _value.ToString();
            return any;
        }

        public static Field FromLong(long _value)
        {
            Field any = new Field();
            any.tag_ = Tag.LongValue;
            any.value_ = _value.ToString();
            return any;
        }
        public static Field FromStringAry(string[] _value)
        {
            Field any = new Field();
            any.tag_ = Tag.StringAryValue;
            string ary = "";
            foreach(string v in _value)
            {
                ary += string.Format("{0},", v);
            }
            if(!string.IsNullOrEmpty(ary))
            {
                ary = ary.Remove(ary.Length - 1, 1);
            }
            any.value_ = string.Format("[{0}]", ary);
            return any;
        }

        public static Field FromFloatAry(float[] _value)
        {
            Field any = new Field();
            any.tag_ = Tag.FloatAryValue;
            string ary = "";
            foreach (float v in _value)
            {
                ary += string.Format("{0},", v);
            }
            if (!string.IsNullOrEmpty(ary))
            {
                ary = ary.Remove(ary.Length - 1, 1);
            }
            any.value_ = string.Format("[{0}]", ary);
            return any;
        }

        public static Field FromDoubleAry(double[] _value)
        {
            Field any = new Field();
            any.tag_ = Tag.DoubleAryValue;
            string ary = "";
            foreach (double v in _value)
            {
                ary += string.Format("{0},", v);
            }
            if (!string.IsNullOrEmpty(ary))
            {
                ary = ary.Remove(ary.Length - 1, 1);
            }
            any.value_ = string.Format("[{0}]", ary);
            return any;
        }

        public static Field FromBoolAry(bool[] _value)
        {
            Field any = new Field();
            any.tag_ = Tag.BoolAryValue;
            string ary = "";
            foreach (bool v in _value)
            {
                ary += string.Format("{0},", v);
            }
            if (!string.IsNullOrEmpty(ary))
            {
                ary = ary.Remove(ary.Length - 1, 1);
            }
            any.value_ = string.Format("[{0}]", ary);
            return any;
        }

        public static Field FromIntAry(int[] _value)
        {
            Field any = new Field();
            any.tag_ = Tag.IntAryValue;
            string ary = "";
            foreach (int v in _value)
            {
                ary += string.Format("{0},", v);
            }
            if (!string.IsNullOrEmpty(ary))
            {
                ary = ary.Remove(ary.Length - 1, 1);
            }
            any.value_ = string.Format("[{0}]", ary);
            return any;
        }

        public static Field FromLongAry(long[] _value)
        {
            Field any = new Field();
            any.tag_ = Tag.LongAryValue;
            string ary = "";
            foreach (long v in _value)
            {
                ary += string.Format("{0},", v);
            }
            if (!string.IsNullOrEmpty(ary))
            {
                ary = ary.Remove(ary.Length - 1, 1);
            }
            any.value_ = string.Format("[{0}]", ary);
            return any;
        }

        public bool IsNull()
        {
            return tag_ == Tag.NULL;
        }

        public bool IsString()
        {
            return tag_ == Tag.StringValue;
        }

        public bool IsInt()
        {
            return tag_ == Tag.IntValue;
        }

        public bool IsLong()
        {
            return tag_ == Tag.LongValue;
        }

        public bool IsFloat()
        {
            return tag_ == Tag.FloatValue;
        }

        public bool IsDouble()
        {
            return tag_ == Tag.DoubleValue;
        }

        public bool IsBool()
        {
            return tag_ == Tag.BoolValue;
        }

        public bool IsStringAry()
        {
            return tag_ == Tag.StringAryValue;
        }

        public bool IsIntAry()
        {
            return tag_ == Tag.IntAryValue;
        }

        public bool IsLongAry()
        {
            return tag_ == Tag.LongAryValue;
        }

        public bool IsFloatAry()
        {
            return tag_ == Tag.FloatAryValue;
        }

        public bool IsDoubleAry()
        {
            return tag_ == Tag.DoubleAryValue;
        }

        public bool IsBoolAry()
        {
            return tag_ == Tag.BoolAryValue;
        }



        public string AsString()
        {
            return value_;
        }

        public int AsInt()
        {
            int value = 0;
            int.TryParse(value_, out value);
            return value;
        }

        public long AsLong()
        {
            long value = 0;
            long.TryParse(value_, out value);
            return value;
        }

        public float AsFloat()
        {
            float value = 0;
            float.TryParse(value_, out value);
            return value;
        }

        public double AsDouble()
        {
            double value = 0;
            double.TryParse(value_, out value);
            return value;
        }

        public bool AsBool()
        {
            bool value = false;
            bool.TryParse(value_, out value);
            return value;
        }

        public string[] AsStringAry()
        {
            List<string> v = new List<string>();
            if (value_.StartsWith("[") && value_.EndsWith("]"))
            {
                string ary = value_.Remove(0, 1);
                ary = ary.Remove(ary.Length - 1, 1);
                foreach (string e in ary.Split(","))
                {
                    string value = e.Trim();
                    v.Add(value);
                }
            }
            return v.ToArray();
        }

        public int[] AsIntAry()
        {
            List<int> v = new List<int>();
            if (value_.StartsWith("[") && value_.EndsWith("]"))
            {
                string ary = value_.Remove(0, 1);
                ary = ary.Remove(ary.Length - 1, 1);
                foreach (string e in ary.Split(","))
                {
                    int value;
                    if(int.TryParse(e.Trim(), out value))
                        v.Add(value);
                    else
                        v.Add(0);
                }
            }
            return v.ToArray();
        }

        public long[] AsLongAry()
        {
            List<long> v = new List<long>();
            if (value_.StartsWith("[") && value_.EndsWith("]"))
            {
                string ary = value_.Remove(0, 1);
                ary = ary.Remove(ary.Length - 1, 1);
                foreach (string e in ary.Split(","))
                {
                    long value;
                    if (long.TryParse(e.Trim(), out value))
                        v.Add(value);
                    else
                        v.Add(0);
                }
            }
            return v.ToArray();
        }

        public float[] AsFloatAry()
        {
            List<float> v = new List<float>();
            if (value_.StartsWith("[") && value_.EndsWith("]"))
            {
                string ary = value_.Remove(0, 1);
                ary = ary.Remove(ary.Length - 1, 1);
                foreach (string e in ary.Split(","))
                {
                    float value;
                    if (float.TryParse(e.Trim(), out value))
                        v.Add(value);
                    else
                        v.Add(0);
                }
            }
            return v.ToArray();
        }

        public double[] AsDoubleAry()
        {
            List<double> v = new List<double>();
            if (value_.StartsWith("[") && value_.EndsWith("]"))
            {
                string ary = value_.Remove(0, 1);
                ary = ary.Remove(ary.Length - 1, 1);
                foreach (string e in ary.Split(","))
                {
                    double value;
                    if (double.TryParse(e.Trim(), out value))
                        v.Add(value);
                    else
                        v.Add(0);
                }
            }
            return v.ToArray();
        }

        public bool[] AsBoolAry()
        {
            List<bool> v = new List<bool>();
            if (value_.StartsWith("[") && value_.EndsWith("]"))
            {
                string ary = value_.Remove(0, 1);
                ary = ary.Remove(ary.Length - 1, 1);
                foreach (string e in ary.Split(","))
                {
                    bool value;
                    if (bool.TryParse(e.Trim(), out value))
                        v.Add(value);
                    else
                        v.Add(false);
                }
            }
            return v.ToArray();
        }

        public Any AsAny()
        {
            if(IsString())
                return Any.FromString(AsString());
            if(IsInt())
                return Any.FromInt32(AsInt());
            if(IsLong())
                return Any.FromInt64(AsLong());
            if(IsFloat())
                return Any.FromFloat32(AsFloat());
            if(IsDouble())
                return Any.FromFloat64(AsDouble());
            if(IsBool())
                return Any.FromBool(AsBool());
            if (IsStringAry())
                return Any.FromStringAry(AsStringAry());
            if (IsIntAry())
                return Any.FromInt32Ary(AsIntAry());
            if (IsLongAry())
                return Any.FromInt64Ary(AsLongAry());
            if (IsFloatAry())
                return Any.FromFloat32Ary(AsFloatAry());
            if (IsDoubleAry())
                return Any.FromFloat64Ary(AsDoubleAry());
            if (IsBoolAry())
                return Any.FromBoolAry(AsBoolAry());
            return new Any();
        }
    }//class
{{proto}}
}
"""

template_winform_FormRoot_cs = r"""
using XTC.oelMVCS;

namespace {{org}}.Module.{{mod}}
{
    public class FormRoot
    {
        public FormRoot()
        {
        }

        public void Inject(Framework _framework)
        {
            framework_ = _framework;
        }

        public void Register()
        {
{{register}}
        }

        public void Cancel()
        {
{{cancel}}
        }

        private Framework framework_ = null;
    }
}
"""

template_winform_Facade_cs = r"""
using XTC.oelMVCS;

namespace {{org}}.Module.{{mod}}
{
    public class {{service}}Facade : View.Facade
    {
        public const string NAME = "{{mod}}.{{service}}Facade";
    }
}
"""

template_winform_Panel_cs = r"""
using System;
using System.Windows.Forms;
using XTC.oelMVCS;

namespace {{org}}.Module.{{mod}}
{
    public partial class {{service}}Panel : UserControl
    {
        public class {{service}}UiBridge : I{{service}}UiBridge
        {
            public {{service}}Panel panel { get; set; }

            public object getRootPanel()
            {
                return panel;
            }

            public void Alert(string _message)
            {
                MessageBox.Show(_message);
            }
        }

        public {{service}}Facade facade { get; set; }

        public {{service}}Panel()
        {
            InitializeComponent();

            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Location = new System.Drawing.Point(10, 10);
            this.Name = "rootPanel";
            this.TabIndex = 0;
        }
    }
}
"""

template_winform_Panel_resx = r"""
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
</root>
"""

template_winform_Panel_Designer_cs = r"""

namespace {{org}}.Module.{{mod}}
{
    partial class {{service}}Panel
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tcPages = new System.Windows.Forms.TabControl();
            this.tcPages.SuspendLayout();
            this.SuspendLayout();
            //
            // tcPages
            //
            this.tcPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcPages.Location = new System.Drawing.Point(10, 10);
            this.tcPages.Multiline = true;
            this.tcPages.Name = "tcPages";
            this.tcPages.SelectedIndex = 0;
            this.tcPages.Size = new System.Drawing.Size(720, 620);
            this.tcPages.TabIndex = 0;

{{tabPage_block}}

            //
            // {{service}}Panel
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcPages);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "{{Service}}Panel";
            this.Size = new System.Drawing.Size(740, 640);
            this.tcPages.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tcPages;
{{tabPage_define}}
    }
}
"""

##############################################################################
##############################################################################
##############################################################################

org_name = ""
mod_name = ""
layouts: Dict[str, str] = {}
with open('./generator.yml', 'r', encoding='utf-8') as rf:
    yml_str = rf.read()
    rf.close()
    yml_data = yaml.load(yml_str, Loader=yaml.FullLoader)
    org_name = yml_data['org']
    mod_name = yml_data['mod']
    if 'layouts' in yml_data:
        for layout in yml_data['layouts']:
            layouts[layout['method']] = layout['page']


if '' == org_name:
    print('org is empty')
    sys.exit(0)

if '' == mod_name:
    print('mod is empty')
    sys.exit(0)

proto_dir = "./"


services: Dict[str, Dict[str, Tuple]] = {}
"""
service Healthy {
  rpc Echo(EchoRequest) returns (EchoResponse) {}
}
转换格式为
{
    startkit:
    {
        healthy: (EchoRequest, EchoResponse)
    }
}
"""

messages: Dict[str, List[Tuple]] = {}
"""
message EchoRequest {
  string msg = 1;  // 消息
}
转换为
{
    EchoRequest:
    [
        (msg, string, 消息),
    ]
}
"""

enums: List[str] = []

type_dict = {
        "string": "string",
        "int32": "int",
        "uint32": "int",
        "int64": "long",
        "uint64": "long",
        "bool": "bool",
        "float32": "float",
        "float64": "double",
        "bytes": "byte[]",
        "enum": "int",
        }

# 解析协议文件
for entry in os.listdir(proto_dir):
    # 跳过不是.proto的文件
    if not entry.endswith(".proto"):
        continue
    proto_name = os.path.splitext(entry)[0]
    with open(os.path.join(proto_dir, entry), "r", encoding="utf-8") as rf:
        content = rf.read()
        rf.close()
        #############################
        # 提取enum名
        #############################
        match = re.findall(r".*?enum\s*(.*?)\s*\{", content, re.S)
        for enum_name in match:
            enums.append(enum_name)
        #############################
        # 提取service语句块
        #############################
        match = re.findall(r".*?service\s*(.*?\}\s*\})", content, re.S)
        for service_block in match:
            # 提取服务名
            match = re.findall(r"(\w*?)\s*\{", service_block, re.S)
            service_name = match[0]
            services[service_name] = {}
            # 提取服务方法
            for line in str.splitlines(service_block):
                # 跳过不包含rpc的行
                if str.find(line, "rpc") < 0:
                    continue
                # 跳过包含stream的行
                if str.find(line, "stream") >= 0:
                    continue
                # 提取rpc语句块
                match = re.findall(r".*rpc\s*(.*?)\s*\{\s*\}", line, re.S)
                if len(match) == 0:
                    continue
                rpc_block = match[0]
                # 提取方法名
                match = re.findall(r"^(.*?)\s*\(", rpc_block, re.S)
                if len(match) == 0:
                    continue
                rpc_name = match[0]
                # 提取请求
                match = re.findall(r"\(\s*(.*?)\s*\)\s*returns", rpc_block, re.S)
                req_name = ""
                if len(match) > 0:
                    req_name = match[0]
                # 提取回复
                match = re.findall(r"returns\s*\(\s*(.*?)\s*\)", rpc_block, re.S)
                rsp_name = ""
                if len(match) > 0:
                    rsp_name = match[0]
                services[service_name][rpc_name] = (req_name, rsp_name)
        #############################
        # 提取message语句块
        #############################
        match = re.findall(r".*?message\s*(.*?\})", content, re.S)
        for message_block in match:
            # 提取消息名
            match = re.findall(r"(\w*?)\s*\{", message_block, re.S)
            message_name = match[0]
            messages[message_name] = []
            # 提取字段
            for line in str.splitlines(message_block):
                # 跳过不包含=的行
                if str.find(line, "=") < 0:
                    continue
                # 跳过不包含;的行
                if str.find(line, ";") < 0:
                    continue
                isRepeated = False
                if line.find("repeated") >= 0:
                    isRepeated = True
                    line = line.replace("repeated", "")
                # 提取字段类型
                match = re.findall(r"\s*(.*)\s+\w+\s+=", line, re.S)
                field_type = ""
                if len(match) > 0:
                    field_type = match[0]
                if isRepeated:
                    field_type = field_type + "[]"
                if field_type.startswith('map'):
                    field_type = 'System.Collections.Generic.Dictionary' + field_type[3:]
                # 提取字段名
                match = re.findall(r"\s+(\w+)\s+=", line, re.S)
                field_name = ""
                if len(match) > 0:
                    field_name = match[0]
                # 提取字段注释
                match = re.findall(r"//\s+(\w+)", line, re.S)
                field_remark = ""
                if len(match) > 0:
                    field_remark = str.strip(match[0])
                messages[message_name].append((field_name, field_type, field_remark))


# 生成..gitignore文件
os.makedirs("vs2019", exist_ok=True)
with open("./vs2019/.gitignore", "w", encoding="utf-8") as wf:
    wf.write(template_gitignore)
    wf.close()

# 生成.sln文件
os.makedirs("vs2019", exist_ok=True)
with open("./vs2019/{}-{}.sln".format(org_name, mod_name), "w", encoding="utf-8") as wf:
    wf.write(template_sln)
    wf.close()

# -----------------------------------------------------------------------------
# 生成 app 解决方案
# -----------------------------------------------------------------------------
os.makedirs("vs2019/app", exist_ok=True)
# 生成.proj文件
with open("./vs2019/app/app.csproj", "w", encoding="utf-8") as wf:
    wf.write(template_proj_app)
    wf.close()

# 生成AppFacade.cs
with open("./vs2019/app/AppFacade.cs", "w", encoding="utf-8") as wf:
    wf.write(template_app_AppFacade_cs)
    wf.close()

# 生成AppView.cs
with open("./vs2019/app/AppView.cs", "w", encoding="utf-8") as wf:
    wf.write(template_app_AppView_cs)
    wf.close()

# 生成ConsoleLogger.cs
with open("./vs2019/app/ConsoleLogger.cs", "w", encoding="utf-8") as wf:
    wf.write(template_app_ConsoleLogger_cs)
    wf.close()

# 生成AppConfig.cs
with open("./vs2019/app/AppConfig.cs", "w", encoding="utf-8") as wf:
    wf.write(template_app_AppConfig_cs)
    wf.close()


# 生成Program.cs
with open("./vs2019/app/Program.cs", "w", encoding="utf-8") as wf:
    wf.write(
            template_app_Program_cs.replace("{{org}}", org_name.upper()).replace(
                "{{mod}}", mod_name.capitalize()
                )
            )
    wf.close()

# 生成RootForm.cs
with open("./vs2019/app/RootForm.cs", "w", encoding="utf-8") as wf:
    wf.write(template_app_RootForm_cs)
    wf.close()

# 生成RootForm.resx
with open("./vs2019/app/RootForm.resx", "w", encoding="utf-8") as wf:
    wf.write(template_app_RootForm_resx)
    wf.close()

# -----------------------------------------------------------------------------
# 生成 bridge 解决方案
# -----------------------------------------------------------------------------
os.makedirs("vs2019/bridge", exist_ok=True)
# 生成.proj文件
with open("./vs2019/bridge/bridge.csproj", "w", encoding="utf-8") as wf:
    wf.write(
            template_proj_bridge.replace("{{org}}", org_name).replace("{{mod}}", mod_name)
            )
    wf.close()

# 生成ViewBridge.cs文件
for service in services.keys():
    with open(
            "./vs2019/bridge/I{}ViewBridge.cs".format(service), "w", encoding="utf-8"
            ) as wf:
        template_method = r"        void On{{rpc}}Submit({{args}});"
        rpc_block = ""
        for rpc_name in services[service].keys():
            rpc = template_method.replace("{{rpc}}", rpc_name)
            rpc_block = rpc_block + str.format("{}\n", rpc)
            req_name = services[service][rpc_name][0]
            args_block = ""
            for field in messages[req_name]:
                field_name = field[0]
                field_type = field[1]
                # 转换枚举类型
                if field_type in enums:
                    field_type = "enum"
                # 转换类型
                if field_type in type_dict.keys():
                    field_type = type_dict[field_type]
                args_block = args_block + str.format("{} _{}, ", field_type, field_name)
            # 移除末尾的 ', '
            if len(args_block) > 0:
                args_block = args_block[0:-2]
            rpc_block = rpc_block.replace("{{args}}", args_block)
        code = templete_bridge_view_cs
        code = code.replace("{{org}}", org_name.upper())
        code = code.replace("{{mod}}", mod_name.capitalize())
        code = code.replace("{{service}}", service)
        code = code.replace("{{rpc}}", rpc_block)
        wf.write(code)
        wf.close()

# 生成UiBridge.cs文件
for service in services.keys():
    with open(
            "./vs2019/bridge/I{}UiBridge.cs".format(service), "w", encoding="utf-8"
            ) as wf:
        code = templete_bridge_ui_cs
        code = code.replace("{{org}}", org_name.upper())
        code = code.replace("{{mod}}", mod_name.capitalize())
        code = code.replace("{{service}}", service)
        wf.write(code)
        wf.close()


# -----------------------------------------------------------------------------
# 生成 module 解决方案
# -----------------------------------------------------------------------------
os.makedirs("vs2019/module", exist_ok=True)
# 生成.proj文件
with open("./vs2019/module/module.csproj", "w", encoding="utf-8") as wf:
    wf.write(
            template_proj_module.replace("{{org}}", org_name).replace("{{mod}}", mod_name)
            )
    wf.close()


# 生成ModuleRoot.cs文件
with open("./vs2019/module/ModuleRoot.cs", "w", encoding="utf-8") as wf:
    template_register_block = r"""
            // 注册数据层
            framework_.getStaticPipe().RegisterModel({{service}}Model.NAME, new {{service}}Model());
            // 注册视图层
            framework_.getStaticPipe().RegisterView({{service}}View.NAME, new {{service}}View());
            // 注册控制层
            framework_.getStaticPipe().RegisterController({{service}}Controller.NAME, new {{service}}Controller());
            // 注册服务层
            framework_.getStaticPipe().RegisterService({{service}}Service.NAME, new {{service}}Service());
    """
    template_cancel_block = r"""
            // 注销服务层
            framework_.getStaticPipe().CancelService({{service}}Service.NAME);
            // 注销控制层
            framework_.getStaticPipe().CancelController({{service}}Controller.NAME);
            // 注销视图层
            framework_.getStaticPipe().CancelView({{service}}View.NAME);
            // 注销数据层
            framework_.getStaticPipe().CancelModel({{service}}Model.NAME);
    """
    register_block = ""
    cancel_block = ""
    for service in services.keys():
        register_block = register_block + template_register_block.replace(
                "{{service}}", service
                )
        cancel_block = cancel_block + template_cancel_block.replace(
                "{{service}}", service
                )
    code = template_module_ModuleRoot_cs
    code = code.replace("{{org}}", org_name.upper())
    code = code.replace("{{mod}}", mod_name.capitalize())
    code = code.replace("{{register}}", register_block)
    code = code.replace("{{cancel}}", cancel_block)
    wf.write(code)
    wf.close()

# 生成Model.cs文件
for service in services.keys():
    with open(
            "./vs2019/module/{}Model.cs".format(service), "w", encoding="utf-8"
            ) as wf:
        code = template_module_Model_cs
        code = code.replace("{{org}}", org_name.upper())
        code = code.replace("{{mod}}", mod_name.capitalize())
        code = code.replace("{{service}}", service)
        wf.write(code)
        wf.close()

# 生成View.cs文件
for service in services.keys():
    template_router = r"""
           route("/{{org}}/{{mod}}/{{service}}/{{rpc}}", this.handle{{service}}{{rpc}});
    """
    template_handler = r"""
        private void handle{{service}}{{rpc}}(Model.Status _status, object _data)
        {
            var rsp = (Proto.{{rsp}})_data;
            if(rsp._status._code.AsInt() == 0)
                bridge.Alert("Success");
            else
                bridge.Alert(string.Format("Failure：\n\nCode: {0}\nMessage:\n{1}", rsp._status._code.AsInt(), rsp._status._message.AsString()));
        }
    """
    with open("./vs2019/module/{}View.cs".format(service), "w", encoding="utf-8") as wf:
        router_block = ''
        handler_block = ''
        for rpc_name in services[service].keys():
            rsp_name = services[service][rpc_name][1]
            router_block = router_block + template_router.replace("{{org}}", org_name).replace("{{mod}}", mod_name).replace("{{service}}", service).replace("{{rpc}}", rpc_name)
            handler_block = handler_block + template_handler.replace("{{service}}", service).replace("{{rpc}}", rpc_name).replace("{{rsp}}", rsp_name)

        code = template_module_View_cs
        code = code.replace("{{org}}", org_name.upper())
        code = code.replace("{{mod}}", mod_name.capitalize())
        code = code.replace("{{service}}", service)
        code = code.replace("{{routers}}", router_block)
        code = code.replace("{{handlers}}", handler_block)
        wf.write(code)
        wf.close()

# 生成Controller.cs文件
for service in services.keys():
    with open(
            "./vs2019/module/{}Controller.cs".format(service), "w", encoding="utf-8"
            ) as wf:
        code = template_module_Controller_cs
        code = code.replace("{{org}}", org_name.upper())
        code = code.replace("{{mod}}", mod_name.capitalize())
        code = code.replace("{{service}}", service)
        wf.write(code)
        wf.close()

# 生成ViewBridge.cs文件
for service in services.keys():
    with open(
            "./vs2019/module/{}ViewBridge.cs".format(service), "w", encoding="utf-8"
            ) as wf:
        template_method = r"""
        public void On{{rpc}}Submit({{args}})
        {
            Proto.{{req}} req = new Proto.{{req}}();
{{assign}}
            service.Post{{rpc}}(req);
        }
        """
        rpc_block = ""
        for rpc_name in services[service].keys():
            req_name = services[service][rpc_name][0]
            rpc = template_method.replace("{{rpc}}", rpc_name)
            rpc = rpc.replace("{{req}}", req_name)
            rpc_block = rpc_block + str.format("{}\n", rpc)
            args_block = ""
            assign_block = ""
            for field in messages[req_name]:
                field_name = field[0]
                field_type = field[1]
                # 转换枚举类型
                if field_type in enums:
                    field_type = "enum"
                # 转换类型
                if field_type in type_dict.keys():
                    field_type = type_dict[field_type]
                args_block = args_block + str.format("{} _{}, ", field_type, field_name)

                if field_type in type_dict.values():
                    assign_block = assign_block + str.format(
                        "            req._{} = Proto.Field.From{}(_{});\n", field_name, field_type.capitalize(), field_name
                        )
                elif field_type.endswith('[]'):
                    assign_block = assign_block + str.format(
                            "            req._{} = Proto.Field.From{}Ary(_{});\n", field_name, field_type[:-2].capitalize(), field_name
                        )
            # 移除末尾的 ', '
            if len(args_block) > 0:
                args_block = args_block[0:-2]
            rpc_block = rpc_block.replace("{{args}}", args_block)
            rpc_block = rpc_block.replace("{{assign}}", assign_block)
        code = template_module_ViewBridge_cs
        code = code.replace("{{org}}", org_name.upper())
        code = code.replace("{{mod}}", mod_name.capitalize())
        code = code.replace("{{service}}", service)
        code = code.replace("{{rpc}}", rpc_block)
        wf.write(code)
        wf.close()


# 生成Service.cs文件
for service in services.keys():
    with open(
            "./vs2019/module/{}Service.cs".format(service), "w", encoding="utf-8"
            ) as wf:
        template_method = r"""
        public void Post{{rpc}}(Proto.{{req}} _request)
        {
            Dictionary<string, Any> paramMap = new Dictionary<string, Any>();
{{assign}}
            post(string.Format("{0}/{{org}}/{{mod}}/{{service}}/{{rpc}}", getConfig()["domain"].AsString()), paramMap, (_reply) =>
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new FieldConverter());
                var rsp = JsonSerializer.Deserialize<Proto.{{rsp}}>(_reply, options);
                {{service}}Model.{{service}}Status status = Model.Status.New<{{service}}Model.{{service}}Status>(rsp._status._code.AsInt(), rsp._status._message.AsString());
                model.Broadcast("/{{org}}/{{mod}}/{{service}}/{{rpc}}", rsp);
            }, (_err) =>
            {
                getLogger().Error(_err.getMessage());
            }, null);
        }
        """
        rpc_block = ""
        for rpc_name in services[service].keys():
            rpc = template_method.replace("{{org}}", org_name)
            rpc = rpc.replace("{{mod}}", mod_name)
            rpc = rpc.replace("{{service}}", service)
            rpc = rpc.replace("{{rpc}}", rpc_name)
            rpc = rpc.replace("{{req}}", services[service][rpc_name][0])
            rpc = rpc.replace("{{rsp}}", services[service][rpc_name][1])
            rpc_block = rpc_block + str.format("{}\n", rpc)
            req_name = services[service][rpc_name][0]
            assign_block = ""
            for field in messages[req_name]:
                field_name = field[0]
                field_type = field[1]
                # 转换枚举类型
                if field_type in enums:
                    field_type = "enum"
                # 转换类型
                if field_type in type_dict.keys():
                    field_type = type_dict[field_type]

                if field_type in type_dict.values():
                    assign_block = assign_block + str.format(
                        '            paramMap["{}"] = _request._{}.AsAny();\n',
                        field_name,
                        field_name,
                        )
                elif field_type.endswith('[]'):
                    assign_block = assign_block + str.format(
                        '            paramMap["{}"] = _request._{}.AsAny();\n',
                        field_name,
                        field_name,
                        )
            rpc_block = rpc_block.replace("{{assign}}", assign_block)
        code = template_module_Service_cs
        code = code.replace("{{org}}", org_name.upper())
        code = code.replace("{{mod}}", mod_name.capitalize())
        code = code.replace("{{service}}", service)
        code = code.replace("{{rpc}}", rpc_block)
        wf.write(code)
        wf.close()

# 生成Proto.cs文件
with open("./vs2019/module/Protocol.cs", "w", encoding="utf-8") as wf:
    template_class = r"""
        public class {{message}}
        {
            public {{message}}()
            {
{{assign}}
            }
{{field}}
        }
    """
    proto_block = ""
    for message_name in messages.keys():
        field_block = ""
        assign_block = ""
        for field in messages[message_name]:
            field_name = field[0]
            field_type = field[1]
            # 转换枚举类型
            if field_type in enums:
                field_type = "enum"
            # 转换类型
            isArray = False
            if field_type.endswith('[]'):
                field_type = field_type[:-2]
                isArray = True
                if field_type in type_dict.keys():
                    field_type = 'Field'

            else:
                if field_type in type_dict.keys():
                    field_type = 'Field'

            if isArray and field_type != 'Field':
                assign_block = assign_block + str.format(
                        "                _{} = new {}[0];\n", field_name, field_type
                        )
                field_block = field_block + str.format(
                        "            [JsonPropertyName(\"{}\")]\n            public {}[] _{} {{get;set;}}\n", field_name, field_type, field_name
                        )
            else:
                assign_block = assign_block + str.format(
                        "                _{} = new {}();\n", field_name, field_type
                        )
                field_block = field_block + str.format(
                        "            [JsonPropertyName(\"{}\")]\n            public {} _{} {{get;set;}}\n", field_name, field_type, field_name
                        )
        message_block = template_class.replace("{{message}}", message_name)
        message_block = message_block.replace("{{field}}", field_block)
        message_block = message_block.replace("{{assign}}", assign_block)
        proto_block = proto_block + message_block
    code = template_module_Protocol_cs
    code = code.replace("{{org}}", org_name.upper())
    code = code.replace("{{mod}}", mod_name.capitalize())
    code = code.replace("{{proto}}", proto_block)
    wf.write(code)
    wf.close()

# 生成JsonConvert.cs文件
with open("./vs2019/module/JsonConvert.cs", "w", encoding="utf-8") as wf:
    code = template_module_Json_Convert_cs
    code = code.replace("{{org}}", org_name.upper())
    code = code.replace("{{mod}}", mod_name.capitalize())
    wf.write(code)
    wf.close()



# -----------------------------------------------------------------------------
# 生成 winform 解决方案
# -----------------------------------------------------------------------------
os.makedirs("vs2019/winform", exist_ok=True)
# 生成.proj文件
with open("./vs2019/winform/winform.csproj", "w", encoding="utf-8") as wf:
    wf.write(
            template_proj_winform.replace("{{org}}", org_name).replace("{{mod}}", mod_name)
            )
    wf.close()

# 生成FormRoot.cs文件
with open("./vs2019/winform/FormRoot.cs", "w", encoding="utf-8") as wf:
    template_register_block = r"""
            // 注册UI装饰
            {{service}}Facade facade{{service}} = new {{service}}Facade();
            framework_.getStaticPipe().RegisterFacade({{service}}Facade.NAME, facade{{service}});
            {{service}}Panel panel{{service}} = new {{service}}Panel();
            panel{{service}}.facade = facade{{service}};
            {{service}}Panel.{{service}}UiBridge ui{{service}}Bridge = new {{service}}Panel.{{service}}UiBridge();
            ui{{service}}Bridge.panel = panel{{service}};
            facade{{service}}.setUiBridge(ui{{service}}Bridge);
        """
    template_cancel_block = r"""
            // 注销UI装饰
            framework_.getStaticPipe().CancelFacade({{service}}Facade.NAME);
        """
    register_block = ""
    cancel_block = ""
    for service in services.keys():
        register_block = register_block + template_register_block.replace(
                "{{service}}", service
                )
        cancel_block = cancel_block + template_cancel_block.replace(
                "{{service}}", service
                )
    code = template_winform_FormRoot_cs
    code = code.replace("{{org}}", org_name.upper())
    code = code.replace("{{mod}}", mod_name.capitalize())
    code = code.replace("{{register}}", register_block)
    code = code.replace("{{cancel}}", cancel_block)
    wf.write(code)
    wf.close()

# 生成Facade.cs文件
for service in services.keys():
    with open(
            "./vs2019/winform/{}Facade.cs".format(service), "w", encoding="utf-8"
            ) as wf:
        code = template_winform_Facade_cs
        code = code.replace("{{org}}", org_name.upper())
        code = code.replace("{{mod}}", mod_name.capitalize())
        code = code.replace("{{service}}", service)
        wf.write(code)
        wf.close()

# 生成Panel.cs文件
for service in services.keys():
    with open(
            "./vs2019/winform/{}Panel.cs".format(service), "w", encoding="utf-8"
            ) as wf:
        code = template_winform_Panel_cs
        code = code.replace("{{org}}", org_name.upper())
        code = code.replace("{{mod}}", mod_name.capitalize())
        code = code.replace("{{service}}", service)
        wf.write(code)
        wf.close()

# 生成Panel.resx文件
for service in services.keys():
    with open(
            "./vs2019/winform/{}Panel.resx".format(service), "w", encoding="utf-8"
            ) as wf:
        code = template_winform_Panel_resx
        wf.write(code)
        wf.close()

# 生成Panel.Designer.cs文件
for service in services.keys():
    template_tabpage_define = r"""
        private System.Windows.Forms.TabPage tabPage{{rpc}};
    """
    template_submit_button_define = r"""
        private System.Windows.Forms.Button btnSubmit{{rpc}};
        private void btnSubmit{{rpc}}_Click(object sender, System.EventArgs e)
        {
            I{{service}}ViewBridge bridge =  facade.getViewBridge() as I{{service}}ViewBridge;
            {{type_parse}}
            bridge.On{{rpc}}Submit({{args}});
        }
    """
    template_page_define ={
            "PostForm": r"""
        private System.Windows.Forms.TableLayoutPanel tlp{{rpc}};
        private System.Windows.Forms.Label label_{{rpc}}_name;
        private System.Windows.Forms.Label label_{{rpc}}_value;
        private System.Windows.Forms.Label label_{{rpc}}_remark;
    """,
    }
    template_postform_label = r"""
        private System.Windows.Forms.Label label_{{rpc}}_{{field}}_{{type}};
    """
    template_postform_textbox = r"""
        private System.Windows.Forms.TextBox tb_{{rpc}}_{{field}};
    """
    template_tabpage_block = r"""
            //
            // tabPage{{rpc}}
            //
            this.tabPage{{rpc}} = new System.Windows.Forms.TabPage();
            this.tabPage{{rpc}}.Location = new System.Drawing.Point(4, 48);
            this.tabPage{{rpc}}.Name = "{{rpc}}";
            this.tabPage{{rpc}}.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage{{rpc}}.Size = new System.Drawing.Size(459, 373);
            this.tabPage{{rpc}}.TabIndex = 0;
            this.tabPage{{rpc}}.Text = "{{rpc}}";
            this.tabPage{{rpc}}.UseVisualStyleBackColor = true;
            this.tcPages.Controls.Add(tabPage{{rpc}});
    """
    template_submit_button_block = r"""
            //
            // btnSubmit{{rpc}}
            //
            this.btnSubmit{{rpc}} = new System.Windows.Forms.Button();
            this.btnSubmit{{rpc}}.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSubmit{{rpc}}.Location = new System.Drawing.Point(17, 300);
            this.btnSubmit{{rpc}}.Name = "btnSubmit{{rpc}}";
            this.btnSubmit{{rpc}}.Size = new System.Drawing.Size(120, 29);
            this.btnSubmit{{rpc}}.TabIndex = 0;
            this.btnSubmit{{rpc}}.Text = "Submit";
            this.btnSubmit{{rpc}}.UseVisualStyleBackColor = true;
            this.btnSubmit{{rpc}}.Click += new System.EventHandler(this.btnSubmit{{rpc}}_Click);
            this.tabPage{{rpc}}.Controls.Add(this.btnSubmit{{rpc}});
    """
    template_page_block = {
            "PostForm": r"""
            //
            // tlp{{rpc}}
            //
            this.tlp{{rpc}} = new System.Windows.Forms.TableLayoutPanel();
            this.tlp{{rpc}}.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlp{{rpc}}.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlp{{rpc}}.ColumnCount = 3;
            this.tlp{{rpc}}.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp{{rpc}}.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tlp{{rpc}}.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tlp{{rpc}}.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlp{{rpc}}.Location = new System.Drawing.Point(17, 15);
            this.tlp{{rpc}}.Name = "tlp";
            this.tlp{{rpc}}.RowCount = {{field_count}};
            this.tlp{{rpc}}.Size = new System.Drawing.Size(400, 260);
            this.tlp{{rpc}}.TabIndex = 1;
            this.tabPage{{rpc}}.Controls.Add(this.tlp{{rpc}});

            this.label_{{rpc}}_name = new System.Windows.Forms.Label();
            this.label_{{rpc}}_name.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_{{rpc}}_name.AutoSize = true;
            this.label_{{rpc}}_name.Location = new System.Drawing.Point(3, 113);
            this.label_{{rpc}}_name.Name = "label_{{rpc}}_name";
            this.label_{{rpc}}_name.Size = new System.Drawing.Size(43, 17);
            this.label_{{rpc}}_name.TabIndex = 0;
            this.label_{{rpc}}_name.Text = "参数名";
            this.tlp{{rpc}}.Controls.Add(this.label_{{rpc}}_name, 0, 0);

            this.label_{{rpc}}_value = new System.Windows.Forms.Label();
            this.label_{{rpc}}_value.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_{{rpc}}_value.AutoSize = true;
            this.label_{{rpc}}_value.Location = new System.Drawing.Point(3, 113);
            this.label_{{rpc}}_value.Name = "label_{{rpc}}_value";
            this.label_{{rpc}}_value.Size = new System.Drawing.Size(43, 17);
            this.label_{{rpc}}_value.TabIndex = 0;
            this.label_{{rpc}}_value.Text = "参数值";
            this.tlp{{rpc}}.Controls.Add(this.label_{{rpc}}_value, 1, 0);

            this.label_{{rpc}}_remark= new System.Windows.Forms.Label();
            this.label_{{rpc}}_remark.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_{{rpc}}_remark.AutoSize = true;
            this.label_{{rpc}}_remark.Location = new System.Drawing.Point(3, 113);
            this.label_{{rpc}}_remark.Name = "label_{{rpc}}_name";
            this.label_{{rpc}}_remark.Size = new System.Drawing.Size(43, 17);
            this.label_{{rpc}}_remark.TabIndex = 0;
            this.label_{{rpc}}_remark.Text = "说明";
            this.tlp{{rpc}}.Controls.Add(this.label_{{rpc}}_remark, 2, 0);
            """
            }
    template_label_block = r"""
            //
            // label_{{rpc}}_{{field}}
            //
            this.label_{{rpc}}_{{field}}_{{type}} = new System.Windows.Forms.Label();
            this.label_{{rpc}}_{{field}}_{{type}}.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_{{rpc}}_{{field}}_{{type}}.AutoSize = true;
            this.label_{{rpc}}_{{field}}_{{type}}.Location = new System.Drawing.Point(3, 113);
            this.label_{{rpc}}_{{field}}_{{type}}.Name = "label_{{rpc}}_{{field}}";
            this.label_{{rpc}}_{{field}}_{{type}}.Size = new System.Drawing.Size(43, 17);
            this.label_{{rpc}}_{{field}}_{{type}}.TabIndex = 0;
            this.label_{{rpc}}_{{field}}_{{type}}.Text = "{{text}}";
            this.tlp{{rpc}}.Controls.Add(this.label_{{rpc}}_{{field}}_{{type}}, {{column}}, {{row}});
    """
    template_textbox_block = r"""
            //
            // tb_{{rpc}}_{{field}}
            //
            this.tb_{{rpc}}_{{field}} = new System.Windows.Forms.TextBox();
            this.tb_{{rpc}}_{{field}}.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_{{rpc}}_{{field}}.Location = new System.Drawing.Point(189, 50);
            this.tb_{{rpc}}_{{field}}.Name = "tb_{{rpc}}_{{field}}";
            this.tb_{{rpc}}_{{field}}.Size = new System.Drawing.Size(363, 23);
            this.tb_{{rpc}}_{{field}}.TabIndex = 1;
            this.tlp{{rpc}}.Controls.Add(this.tb_{{rpc}}_{{field}}, 1, {{row}});
            this.tlp{{rpc}}.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
    """
    template_type_parse = r"""
            {{field_type}} _{{field_name}};
            {{field_type}}.TryParse({{value}}, out _{{field_name}});
    """
    template_assign = r"""
            {{type}} _{{name}} = {{value}};
    """
    with open(
            "./vs2019/winform/{}Panel.Designer.cs".format(service), "w", encoding="utf-8"
            ) as wf:
        tabPage_define = ''
        tabPage_block = ''
        for rpc_name in services[service]:
            req_name = services[service][rpc_name][0]
            rsp_name = services[service][rpc_name][0]
            tabPage_define = tabPage_define + template_tabpage_define.replace('{{rpc}}', rpc_name)
            tabPage_block = tabPage_block + template_tabpage_block.replace('{{rpc}}', rpc_name)
            #生成页面
            rpc_method = '{}.{}'.format(service, rpc_name)
            if rpc_method in layouts.keys():
                page = layouts[rpc_method]
                if page == 'PostForm':
                    tabPage_define = tabPage_define + template_page_define[page].replace('{{rpc}}', rpc_name)
                    # 多添加一个空位尾行
                    tabPage_block = tabPage_block + template_page_block[page].replace('{{rpc}}', rpc_name).replace("{{field_count}}", str(len(messages[req_name])+2))
                    row = 0
                    args_exp = ''
                    type_parse_block = ''
                    for field in messages[req_name]:
                        field_name = field[0]
                        field_type = field[1]
                        field_remark = field[2]
                        # 字段名声明
                        tabPage_define = tabPage_define + template_postform_label.replace('{{field}}', field_name).replace("{{type}}", "name").replace("{{rpc}}", rpc_name)
                        # 字段注释声明
                        tabPage_define = tabPage_define + template_postform_label.replace('{{field}}', field_name).replace("{{type}}", "remark").replace("{{rpc}}", rpc_name)
                        # 字段输入声明
                        tabPage_define = tabPage_define + template_postform_textbox.replace('{{field}}', field_name).replace("{{rpc}}", rpc_name)
                        # 字段名代码
                        tabPage_block = tabPage_block + template_label_block.replace('{{rpc}}', rpc_name).replace("{{field}}", field_name).replace("{{row}}", str(row+1)).replace("{{column}}", str(0)).replace("{{type}}", "name").replace("{{text}}", field_name)
                        # 字段注释代码
                        tabPage_block = tabPage_block + template_label_block.replace('{{rpc}}', rpc_name).replace("{{field}}", field_name).replace("{{row}}", str(row+1)).replace("{{column}}", str(2)).replace("{{type}}", "remark").replace("{{text}}", field_remark)
                        # 字段输入代码
                        tabPage_block = tabPage_block + template_textbox_block.replace('{{rpc}}', rpc_name).replace("{{field}}", field_name).replace("{{row}}", str(row+1))
                        # 构建参数列表
                        if field_type in enums:
                            field_type = 'enum'
                        if field_type in type_dict.keys():
                            field_type = type_dict[field_type]
                        if 'string' == field_type:
                            type_parse_block = type_parse_block + template_assign.replace('{{type}}', field_type).replace("{{name}}", field_name).replace("{{value}}",'this.tb_{}_{}.Text'.format(rpc_name, field_name))
                        else:
                            if field_type in type_dict.values():
                                type_parse_block = type_parse_block +template_type_parse.replace('{{field_type}}', field_type).replace('{{field_name}}', field_name).replace('{{value}}', 'this.tb_{}_{}.Text'.format(rpc_name, field_name))
                            else:
                                type_parse_block = type_parse_block + str.format('            {} _{} = null;\n            //TODO 未实现的字段 \n', field_type, field_name)
                        args_exp = args_exp + '_{}, '.format(field_name)
                        row=row+1
                    # 移除末尾得', '
                    if len(messages[req_name]) > 0:
                        args_exp = args_exp[:-2]
                    # 提交按钮声明
                    tabPage_define = tabPage_define + template_submit_button_define.replace('{{rpc}}', rpc_name).replace("{{service}}", service).replace("{{args}}", args_exp).replace("{{type_parse}}", type_parse_block)
                    # 提交按钮代码
                    tabPage_block = tabPage_block + template_submit_button_block.replace('{{rpc}}', rpc_name)

        code = template_winform_Panel_Designer_cs
        code = code.replace("{{org}}", org_name.upper())
        code = code.replace("{{mod}}", mod_name.capitalize())
        code = code.replace("{{service}}", service)
        code = code.replace("{{tabPage_define}}", tabPage_define)
        code = code.replace("{{tabPage_block}}", tabPage_block)
        wf.write(code)
        wf.close()
