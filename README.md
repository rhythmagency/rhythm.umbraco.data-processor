The Rhythm Data Processor is an Umbraco dashboard that is easily extended to add new data processing capabilities.

When the dashboard loads, it scans all assemblies for classes marked as data processors, and displays those in a dropdown.

When the user selects a data processor, input controls are shown so the user can enter data, and then that data is passed to the data processor.

To create your own data processor, all you must do is create a public class with a special attribute and two static methods, as shown in this example:

    [Rhythm.DataProcessor.UmbracoDataProcessor(Label = "Your Custom Data Processor")]
    public static class YourCustomDataProcessor
    {
        public static Rhythm.DataProcessor.ProcessorInput[] Inputs()
        {
            return new []
            {
                new Rhythm.DataProcessor.ProcessorInput { Kind = "Node", Label = "Pick a Node" }
            };
        }
        public static string ProcessInputs(List<object> inputs)
        {
            var nodeId = (int)inputs[0];
            // TODO: Your code here.
            return "All went according to plan.";
        }
    }

Once that is done, you will see your data processor appear in the dashboard:
![Dashboard Screenshot](docs/images/dashboard.png?raw=true "Dashboard")

For this example, selecting your data processor would display this:
![Data Processor Screenshot](docs/images/processor.png?raw=true "Data Processor")