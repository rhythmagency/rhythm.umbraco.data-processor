namespace Rhythm.DataProcessor
{

    // Namespaces.
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Umbraco.Core;


    #region class ProcessorKinds

    /// <summary>
    /// The kinds of data processor inputs.
    /// </summary>
    /// <remarks>
    /// Node - Renders a multi-node tree picker. A user must pick exactly one node.
    /// Nodes - Renders a multi-nod tree picker. A user must pick at least one node.
    /// </remarks>
    public class ProcessorKinds
    {
        public const string Node = "Node";
        public const string Nodes = "Nodes";
    }

    #endregion


    #region class UmbracoDataProcessor

    /// <summary>
    /// The class to be used as an attribute that indicates a class is a data processor.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class UmbracoDataProcessor : System.Attribute
    {

        /// <summary>
        /// The label used when displaying the data processor to the user.
        /// </summary>
        public string Label {get; set; }

    }

    #endregion


    #region class ProcessorInput

    /// <summary>
    /// Stores information about the type of input used by a data processor.
    /// </summary>
    public class ProcessorInput
    {

        /// <summary>
        /// The kind of input (e.g., "Node" or "Nodes").
        /// </summary>
        public string Kind { get; set; }


        /// <summary>
        /// The label shown above the input field.
        /// </summary>
        public string Label { get; set; }

    }

    #endregion


    #region class BulkMoveNodes

    /// <summary>
    /// A data processor that moves multiple nodes to under a new parent node.
    /// </summary>
    [UmbracoDataProcessor(Label = "Bulk Move Nodes")]
    public static class BulkMoveNodes
    {

        /// <summary>
        /// The types of inputs required by this data processor.
        /// </summary>
        /// <returns>The input types.</returns>
        public static ProcessorInput[] Inputs()
        {
            return new []
            {
                new ProcessorInput { Kind = "Nodes", Label = "Select Nodes to Move" },
                new ProcessorInput { Kind = "Node", Label = "Select Destination Node" }
            };
        }


        /// <summary>
        /// Processes the input data.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns>
        /// A string containing markup that is shown to the user.
        /// This will indicate success or failure.
        /// </returns>
        public static string ProcessInputs(List<object> inputs)
        {

            // Variables.
            var sources = inputs[0] as int[];
            var destination = (int)inputs[1];
            var service = ApplicationContext.Current.Services.ContentService;
            var destinationNode = service.GetById(destination);
            var removeEmpties = StringSplitOptions.RemoveEmptyEntries;
            var comma = ",".ToCharArray();
            var ancestors = destinationNode.Path.Split(comma, removeEmpties)
                .Select(x => int.Parse(x)).ToArray();


            // Ensure the destination node isn't under any of the source nodes.
            if (ancestors.Any(x => sources.Contains(x)))
            {
                return "<h2>Error</h2><br />The destination node cannot be one of or reside under any of the source nodes.";
            }


            // Move each node.
            foreach (var source in sources)
            {
                service.Move(service.GetById(source), destination);
            }


            // Refresh the XML cache.
            umbraco.library.RefreshContent();


            // Indicate success.
            return string.Format("<h2>Success</h2>Moved {0} nodes.", sources.Length.ToString());

        }

    }

    #endregion

}