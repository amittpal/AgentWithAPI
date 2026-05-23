using System.ComponentModel;

namespace AgentAPI.AgentTools
{
    public class StudentTool
    {
        private static List<string> studentNames = new();

        [Description("Gets all the students name from the studentNames list")]
        public string[] GetStudents()
        {
            return studentNames.ToArray();
        }

        [Description("Adds a new student name to the studentNames list")]
        public string AddStudent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Error: Student name cannot be empty.";
            }
            
            if (studentNames.Contains(name))
            {
                return $"Error: Student '{name}' already exists.";
            }
            
            studentNames.Add(name);
            return $"Student '{name}' added successfully.";
        }

        [Description("Deletes a student name from the studentNames list")]
        public string DeleteStudent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Error: Student name cannot be empty.";
            }
            
            if (studentNames.Remove(name))
            {
                return $"Student '{name}' deleted successfully.";
            }
            
            return $"Error: Student '{name}' not found.";
        }
    }
}
