namespace Rhythm.DataProcessor
{

    // Namespaces.
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;


    /// <summary>
    /// The dashboard for the data processor.
    /// </summary>
    public partial class DataProcessorDashboard : System.Web.UI.UserControl
    {

        #region Event Handlers

        /// <summary>
        /// Page load.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var labels = new List<string>() { "Select a Processor..." };
                labels.AddRange(GetDataProcessors().Select(x => x.Item2.Label));
                ddlProcessors.DataSource = labels;
                ddlProcessors.DataBind();
            }
        }


        /// <summary>
        /// "Process" was clicked.
        /// </summary>
        protected void btnProcess_Click(object sender, EventArgs e)
        {

            // Variables.
            var valid = true;
            var inputs = new List<object>();
            var lines = new List<string>();
            var containers = new [] { "phNode", "phNodes" };


            // Grab and validate data from each input.
            foreach(RepeaterItem item in rpInputs.Items)
            {

                // Get input control.
                var inputControl = containers.Select(x => item.FindControl(x))
                    .Where(x => x != null && x.Visible).First().Controls[0];


                // Input is a multi-node tree picker?
                if(inputControl.ID == "inputNode" || inputControl.ID == "inputNodes")
                {

                    // Variables.
                    var mntp = inputControl as umbraco.editorControls.MultiNodeTreePicker.MNTP_DataEditor;
                    var ids = mntp.SelectedIds;


                    // Single node?
                    if(inputControl.ID == "inputNode")
                    {
                        if(ids.Length == 1)
                        {
                            inputs.Add(int.Parse(ids.First()));
                        }
                        else
                        {
                            valid = false;
                        }
                    }


                    // Multiple nodes.
                    if(inputControl.ID == "inputNodes")
                    {
                        if(ids.Length > 0)
                        {
                            inputs.Add(ids.Select(x => int.Parse(x)).ToArray());
                        }
                        else
                        {
                            valid = false;
                        }
                    }

                }

            }


            // Were all of the inputs valid?
            if(valid)
            {

                // Pass the data to the data processor.
                var processor = GetDataProcessors()
                    .First(x => x.Item2.Label == ddlProcessors.SelectedValue);
                litResults.Text = processor.Item1.GetMethod("ProcessInputs")
                    .Invoke(null, new [] {inputs}) as string;
                ddlProcessors.SelectedIndex = 0;

            }
            else
            {

                // Invalid input.
                litResults.Text = "<h2>Error</h2>";

            }

        }


        /// <summary>
        /// "Processors" drop down changed.
        /// </summary>
        protected void ddlProcessors_Change(object sender, EventArgs e)
        {
            litResults.Text = null;
            rpInputs.DataSource = GetInputs();
            rpInputs.DataBind();
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Gets the data processors in the current app domain.
        /// </summary>
        /// <returns>The type and attribute for each data processor.</returns>
        private List<Tuple<Type, UmbracoDataProcessor>> GetDataProcessors()
        {

            // Variables.
            var processors = new List<Tuple<Type, UmbracoDataProcessor>>();


            // Find all types in all assemblies that are marked as data processors.
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {

                // Variables.
                var types = new List<Type>() as IEnumerable<Type>;


                // Dynamic assemblies may throw an exception.
                try
                {
                    types = assembly.ExportedTypes;
                }
                catch { }


                // Look at the attributes on each type.
                foreach(var type in types)
                {
                    var attributes = type.GetCustomAttributes(typeof(UmbracoDataProcessor), false)
                        .Cast<UmbracoDataProcessor>();
                    if(attributes.Any())
                    {
                        var attribute = attributes.First();
                        processors.Add(new Tuple<Type,UmbracoDataProcessor>(type, attribute));
                    }
                }

            }


            // Return data processors.
            return processors;

        }


        /// <summary>
        /// Gets info about the inputs for the selected data processor.
        /// </summary>
        /// <returns>The data processor input information.</returns>
        private ProcessorInput[] GetInputs()
        {
            var processor = GetDataProcessors()
                .First(x => x.Item2.Label == ddlProcessors.SelectedValue);
            return processor.Item1.GetMethod("Inputs").Invoke(null, null) as ProcessorInput[];
        }

        #endregion

    }

}