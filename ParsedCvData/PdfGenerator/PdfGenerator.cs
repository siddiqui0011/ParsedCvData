using System.Collections;
using System.Dynamic;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace ParsedCvData.PdfGenerator
{
    public class PdfGenerator
    {
        public static string GeneratePdf(dynamic candidate)
        {
            string pdfPath = $"{DynamicExtensions.GetProperty(candidate, "Name")?.ToString().Replace(" ", "_") ?? "CV"}.pdf";

            using (var writer = new PdfWriter(pdfPath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new Document(pdf, PageSize.A4);

                    var leftColumn = CreateLeftColumn(candidate);
                    pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new LeftColumnEventHandler(leftColumn));

                    var rightColumn = CreateRightColumn(candidate);
                    document.Add(rightColumn);
                }
            }

            return pdfPath;
        }
        private static Div CreateLeftColumn(dynamic candidate)
        {
            var leftColumn = new Div()
                .SetWidth(200)
                .SetHeight(1000) // Set a fixed height for the left column
                .SetPadding(10)
                .SetBackgroundColor(new DeviceRgb(8, 0, 66))
                .SetBorder(new SolidBorder(new DeviceRgb(255, 255, 255), 1))
                .SetVerticalAlignment(VerticalAlignment.TOP); // Ensure vertical alignment is set to top

            var fontColor = new DeviceRgb(255, 244, 254);

            // Add content to the left column
            AddContactDetails(candidate, leftColumn, fontColor);
            AddEducation(candidate, leftColumn, fontColor);
            AddSkills(candidate, leftColumn, fontColor);
            AddCertifications(candidate, leftColumn, fontColor);

            return leftColumn;
        }
        private static void AddContactDetails(dynamic candidate, Div leftColumn, DeviceRgb fontColor)
        {
            leftColumn.Add(new Paragraph("Contact Details")
                .SetFontSize(16)
                .SetBold()
                .SetFontColor(fontColor)
                .SetMarginBottom(5));

            var phoneNumbers = DynamicExtensions.GetProperty(candidate, "Phone Number");
            if (phoneNumbers != null)
            {
                if (phoneNumbers is IEnumerable<object> phoneArray)
                {
                    foreach (var phone in phoneArray)
                    {
                        leftColumn.Add(new Paragraph($"{phone}").SetFontColor(fontColor));
                    }
                }
                else if (phoneNumbers is string singlePhone)
                {
                    leftColumn.Add(new Paragraph($"{singlePhone}").SetFontColor(fontColor));
                }
            }

            var emails = DynamicExtensions.GetProperty(candidate, "Email");
            if (emails != null)
            {
                if (emails is IEnumerable<object> emailArray)
                {
                    foreach (var email in emailArray)
                    {
                        leftColumn.Add(new Paragraph($"{email}").SetFontColor(fontColor));
                    }
                }
                else if (emails is string singleEmail)
                {
                    leftColumn.Add(new Paragraph($"{singleEmail}").SetFontColor(fontColor));
                }
            }
        }
        private static void AddEducation(dynamic candidate, Div leftColumn, DeviceRgb fontColor)
        {
            leftColumn.Add(new Paragraph("Education").SetFontSize(16).SetBold().SetFontColor(fontColor));
            var education = DynamicExtensions.GetProperty(candidate, "Education");

            if (education != null)
            {
                if (education is IEnumerable<object> eduArray)
                {
                    foreach (var edu in eduArray)
                    {
                        var degree = DynamicExtensions.GetProperty(edu, "Degree")?.ToString();
                        var institution = DynamicExtensions.GetProperty(edu, "Institution")?.ToString();
                        var graduationDate = DynamicExtensions.GetProperty(edu, "Graduation Date")?.ToString();
                        var percentage = DynamicExtensions.GetProperty(edu, "Percentage")?.ToString();

                        var eduText = $"• {degree} from {institution} ({graduationDate})";
                        if (!string.IsNullOrEmpty(percentage))
                        {
                            eduText += $" - {percentage}";
                        }

                        leftColumn.Add(new Paragraph(eduText).SetFontColor(fontColor));
                    }
                }
                else if (education is IDictionary<string, object> eduObject)
                {
                    var degree = DynamicExtensions.GetProperty(eduObject, "Degree")?.ToString();
                    var institution = DynamicExtensions.GetProperty(eduObject, "Institution")?.ToString();
                    var graduationDate = DynamicExtensions.GetProperty(eduObject, "Graduation Date")?.ToString();

                    var eduText = $"• {degree} from {institution} ({graduationDate})";
                    leftColumn.Add(new Paragraph(eduText).SetFontColor(fontColor));
                }
            }
        }
        private static void AddSkills(dynamic candidate, Div leftColumn, DeviceRgb fontColor)
        {
            leftColumn.Add(new Paragraph("Skills").SetFontSize(16).SetBold().SetFontColor(fontColor));
            var skills = DynamicExtensions.GetProperty(candidate, "Skills");
            if (skills != null)
            {
                if (skills is IEnumerable<object> skillsArray)
                {
                    foreach (var skill in skillsArray)
                    {
                        leftColumn.Add(new Paragraph($"• {skill.ToString() ?? "N/A"}").SetFontColor(fontColor));
                    }
                }
                else if (skills is string skillsString)
                {
                    leftColumn.Add(new Paragraph($"• {skillsString}").SetFontColor(fontColor));
                }
            }
        }
        private static void AddCertifications(dynamic candidate, Div leftColumn, DeviceRgb fontColor)
        {
            leftColumn.Add(new Paragraph("Certifications").SetFontSize(16).SetBold().SetFontColor(fontColor));
            var certifications = DynamicExtensions.GetProperty(candidate, "Certifications");
            if (certifications != null)
            {
                if (certifications is IEnumerable<object> certArray)
                {
                    foreach (var cert in certArray)
                    {
                        var certName = DynamicExtensions.GetProperty(cert, "Certification Name")?.ToString();
                        var issuingOrg = DynamicExtensions.GetProperty(cert, "Issuing Organization")?.ToString();
                        var date = DynamicExtensions.GetProperty(cert, "Date")?.ToString();

                        var certText = $"• {certName} by {issuingOrg} ({date})";
                        leftColumn.Add(new Paragraph(certText).SetFontColor(fontColor));
                    }
                }
                else if (certifications is IDictionary<string, object> certObject)
                {
                    var certName = DynamicExtensions.GetProperty(certObject, "Certification Name")?.ToString();
                    var issuingOrg = DynamicExtensions.GetProperty(certObject, "Issuing Organization")?.ToString();
                    var date = DynamicExtensions.GetProperty(certObject, "Date")?.ToString();

                    var certText = $"• {certName} by {issuingOrg} ({date})";
                    leftColumn.Add(new Paragraph(certText).SetFontColor(fontColor));
                }
            }
        }
        private static void HandleCertifications(dynamic certifications, Div leftColumn)
        {
            if (certifications is IEnumerable<object> certArray)
            {
                foreach (var cert in certArray)
                {
                    var certName = DynamicExtensions.GetProperty(cert, "Certification Name")?.ToString() ?? "N/A";
                    var issuer = DynamicExtensions.GetProperty(cert, "Issuing Organization")?.ToString();
                    var issueDate = DynamicExtensions.GetProperty(cert, "Date")?.ToString(); // Adjusted to match "Date"

                    var certText = $"• {certName}";
                    if (!string.IsNullOrEmpty(issuer))
                    {
                        certText += $" (Issued by {issuer})";
                    }
                    if (!string.IsNullOrEmpty(issueDate))
                    {
                        certText += $" - {issueDate}";
                    }

                    leftColumn.Add(new Paragraph(certText));
                }
            }
            else if (certifications is IDictionary<string, object> certObject)
            {
                var certName = DynamicExtensions.GetProperty(certObject, "Certification Name")?.ToString() ?? "N/A";
                var issuer = DynamicExtensions.GetProperty(certObject, "Issuing Organization")?.ToString();
                var issueDate = DynamicExtensions.GetProperty(certObject, "Date")?.ToString(); // Adjusted to match "Date"

                var certText = $"• {certName}";
                if (!string.IsNullOrEmpty(issuer))
                {
                    certText += $" (Issued by {issuer})";
                }
                if (!string.IsNullOrEmpty(issueDate))
                {
                    certText += $" - {issueDate}";
                }

                leftColumn.Add(new Paragraph(certText));
            }
        }
        private static Div CreateRightColumn(dynamic candidate)
        {
            var rightColumn = new Div()
                .SetPadding(10)
                .SetBackgroundColor(new DeviceRgb(255, 255, 255))
                .SetMarginLeft(200) // To position it to the right of the left column
                .SetBorderTop(new SolidBorder(new DeviceRgb(255, 1, 28), 2)) // Red border at the top
                .SetBorderRight(new SolidBorder(new DeviceRgb(255, 1, 28), 2)); // Red border on the right side

            // Add image
            var imagePath = @"C:\Users\ravis\source\repos\ParsedCvData\ParsedCvData\Image\tol.jpg";
            if (File.Exists(imagePath))
            {
                var image = new Image(ImageDataFactory.Create(imagePath));
                image.SetWidth(120) // Increased size
                    .SetMarginTop(-50) // Shifted upwards
                    .SetHorizontalAlignment(HorizontalAlignment.RIGHT); // Align image to the right
                rightColumn.Add(image);
            }

            //// Add LinkedIn URL
            //var linkedinUrl = DynamicExtensions.GetProperty(candidate, "LinkedIn URL")?.ToString();
            //if (!string.IsNullOrEmpty(linkedinUrl))
            //{
            //    var linkedinParagraph = new Paragraph("LinkedIn: ")
            //        .SetFontSize(12)
            //        .SetBold()
            //        .SetMarginTop(10)
            //        .SetTextAlignment(TextAlignment.LEFT); // Changed to LEFT
            //    var linkedinLink = new Paragraph(linkedinUrl)
            //        .SetFontSize(12)
            //        .SetFontColor(ColorConstants.BLUE)
            //        .SetUnderline() // Make URL underlined to indicate it's clickable
            //        .SetTextAlignment(TextAlignment.LEFT); // Changed to LEFT
            //    rightColumn.Add(linkedinParagraph);
            //    rightColumn.Add(linkedinLink);
            //}

            // Add name
            var name = DynamicExtensions.GetProperty(candidate, "Name")?.ToString();
            if (!string.IsNullOrEmpty(name))
            {
                rightColumn.Add(new Paragraph(name)
                    .SetFontSize(30) // Increased font size
                    .SetBold()
                    .SetMarginTop(10)
                    .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT
            }

            // Add professional summary or summary
            var professionalSummary = DynamicExtensions.GetProperty(candidate, "Professional Summary")?.ToString()
                ?? DynamicExtensions.GetProperty(candidate, "Summary")?.ToString();
            if (!string.IsNullOrEmpty(professionalSummary))
            {
                rightColumn.Add(new Paragraph("Professional Summary")
                    .SetFontSize(16)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT
                rightColumn.Add(new Paragraph(professionalSummary)
                    .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT
            }

            // Add Total Years of Experience
            var totalYearsOfExperience = DynamicExtensions.GetProperty(candidate, "Total Year of Experience")?.ToString();
            if (!string.IsNullOrEmpty(totalYearsOfExperience))
            {
                rightColumn.Add(new Paragraph("Total Years of Experience")
                    .SetFontSize(16)
                    .SetBold()
                    .SetMarginTop(10)
                    .SetTextAlignment(TextAlignment.LEFT)); // Changed to LEFT
                rightColumn.Add(new Paragraph($"• {totalYearsOfExperience} Years")
                    .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT
            }

            // Add Experience
            var experience = DynamicExtensions.GetProperty(candidate, "Experience");
            if (experience != null)
            {
                rightColumn.Add(new Paragraph("Experience")
                    .SetFontSize(16)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT

                int experienceNumber = 1;
                foreach (var exp in GetEnumerable(experience))
                {
                    var jobTitle = DynamicExtensions.GetProperty(exp, "Job Title")?.ToString();
                    var company = DynamicExtensions.GetProperty(exp, "Company")?.ToString();
                    var duration = DynamicExtensions.GetProperty(exp, "Duration")?.ToString();
                    var dates = DynamicExtensions.GetProperty(exp, "Dates")?.ToString(); // Added for Dates field

                    if (!string.IsNullOrEmpty(jobTitle))
                    {
                        var jobTitleParagraph = new Paragraph($"{experienceNumber}. {jobTitle}")
                            .SetFontSize(12)
                            .SetBold()
                            .SetMarginTop(5)
                            .SetTextAlignment(TextAlignment.JUSTIFIED); // Changed to LEFT

                        rightColumn.Add(jobTitleParagraph);
                    }

                    if (!string.IsNullOrEmpty(company))
                    {
                        rightColumn.Add(new Paragraph(company)
                            .SetFontSize(12)
                            .SetFontColor(new DeviceRgb(255, 1, 28))
                            .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT
                    }

                    if (!string.IsNullOrEmpty(dates)) // Add Dates field
                    {
                        rightColumn.Add(new Paragraph(dates)
                            .SetFontSize(12)
                            .SetFontColor(ColorConstants.GRAY)
                            .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT
                    }

                    if (!string.IsNullOrEmpty(duration))
                    {
                        rightColumn.Add(new Paragraph(duration)
                            .SetFontSize(12)
                            .SetFontColor(ColorConstants.GRAY)
                            .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT
                    }

                    var responsibilities = DynamicExtensions.GetProperty(exp, "Responsibilities");
                    if (responsibilities != null)
                    {
                        if (responsibilities is IEnumerable<object> respArray)
                        {
                            foreach (var responsibility in respArray)
                            {
                                var resp = responsibility?.ToString();
                                if (!string.IsNullOrEmpty(resp))
                                {
                                    rightColumn.Add(new Paragraph($"• {resp}")
                                        .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT
                                }
                            }
                        }
                        else if (responsibilities is string respStr)
                        {
                            rightColumn.Add(new Paragraph($"• {respStr}")
                                .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT
                        }
                    }

                    experienceNumber++;
                }
            }

            // Add Project Details or Projects
            var projects = DynamicExtensions.GetProperty(candidate, "Projects") ?? DynamicExtensions.GetProperty(candidate, "Project Details");
            if (projects != null)
            {
                rightColumn.Add(new Paragraph("Project Details")
                    .SetFontSize(16)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT

                int projectNumber = 1;
                foreach (var project in GetEnumerable(projects))
                {
                    var projectName = DynamicExtensions.GetProperty(project, "Project Name")?.ToString();
                    var description = DynamicExtensions.GetProperty(project, "Description")?.ToString();
                    var technologiesUsed = DynamicExtensions.GetProperty(project, "Technologies Used");
                    var role = DynamicExtensions.GetProperty(project, "Role")?.ToString();

                    if (!string.IsNullOrEmpty(projectName))
                    {
                        rightColumn.Add(new Paragraph($"{projectNumber}. {projectName}")
                            .SetFontSize(12)
                            .SetBold()
                            .SetMarginTop(5)
                            .SetTextAlignment(TextAlignment.LEFT)); // Changed to LEFT
                    }

                    if (!string.IsNullOrEmpty(description))
                    {
                        rightColumn.Add(new Paragraph($"• {description}")
                            .SetTextAlignment(TextAlignment.LEFT)); // Changed to LEFT
                    }

                    if (technologiesUsed != null)
                    {
                        string techString = null;

                        if (technologiesUsed is IEnumerable<object> techArray)
                        {
                            techString = string.Join(", ", techArray.Select(t => t.ToString()));
                        }
                        else if (technologiesUsed is string techStr)
                        {
                            techString = techStr;
                        }

                        if (!string.IsNullOrWhiteSpace(techString))
                        {
                            rightColumn.Add(new Paragraph($"Technologies Used: {techString}")
                                .SetTextAlignment(TextAlignment.JUSTIFIED)); // Changed to LEFT
                        }
                    }
                    var roles = DynamicExtensions.GetProperty(project, "Role");
                    if (roles != null)
                    {
                        string roleString = null;

                        if (roles is IEnumerable<object> roleArray)
                        {
                            roleString = string.Join(", ", roleArray.Select(r => r.ToString()));
                        }
                        else if (roles is string roleStr)
                        {
                            roleString = roleStr;
                        }

                        if (!string.IsNullOrWhiteSpace(roleString))
                        {
                            rightColumn.Add(new Paragraph($"Role: {roleString}")
                                .SetTextAlignment(TextAlignment.JUSTIFIED)); // Justify role
                        }
                    }

                    projectNumber++;
                }
            }

            return rightColumn;
        }

        // Helper method to handle dynamic IEnumerable
        private static IEnumerable<object> GetEnumerable(dynamic obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The object cannot be null.");
            }

            if (obj is IEnumerable<object> enumerableObject)
            {
                return enumerableObject;
            }
            else if (obj is IEnumerable enumerable)
            {
                return enumerable.Cast<object>();
            }
            else
            {
                throw new InvalidOperationException("The object is not an enumerable.");
            }
        }
        private class LeftColumnEventHandler : IEventHandler
        {
            private readonly Div _leftColumn;

            public LeftColumnEventHandler(Div leftColumn)
            {
                _leftColumn = leftColumn;
            }

            public void HandleEvent(Event currentEvent)
            {
                var docEvent = (PdfDocumentEvent)currentEvent;
                var pdfDoc = docEvent.GetDocument();
                var page = docEvent.GetPage();
                var pdfPageSize = page.GetPageSize();

                // Create a new Canvas to draw on the page
                var pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                var canvas = new Canvas(pdfCanvas, pdfPageSize);

                // Set the position of the left column
                var x = 20; // X-coordinate where left column starts
                var y = 20; // Y-coordinate where left column starts

                // Draw the left column (actually adds the content, not just a rectangle)
                canvas.SetFixedPosition(x, y, 200); // Set fixed position with width of 200
                canvas.Add(_leftColumn);

                canvas.Close();
            }

            private static void AddBorderAndImageToPage(Document document, PdfPage page)
            {
                var canvas = new PdfCanvas(page);
                var pageSize = page.GetPageSize();

                // Draw border line
                canvas
                    .SetStrokeColor(DeviceRgb.BLACK)
                    .SetLineWidth(1)
                    .MoveTo(0, pageSize.GetTop())
                    .LineTo(pageSize.GetRight(), pageSize.GetTop())
                    .LineTo(pageSize.GetRight(), pageSize.GetBottom())
                    .LineTo(0, pageSize.GetBottom())
                    .ClosePathStroke();

                // Add image
                var image = new Image(ImageDataFactory.Create(@"C:\Users\ravis\source\repos\TestingParsedCandidateCV\TestingParsedCandidateCV\Image\tol.jpg"));
                image.SetFixedPosition(10, 10) // Adjust position
                    .SetWidth(pageSize.GetWidth() - 20) // Adjust width
                    .SetHeight(pageSize.GetHeight() - 20); // Adjust height

                document.Add(image);
            }
            private class ImagePageEventHandler : IEventHandler
            {
                private readonly string _imagePath;

                public ImagePageEventHandler(string imagePath)
                {
                    _imagePath = imagePath;
                }

                public void HandleEvent(Event currentEvent)
                {
                    var docEvent = (PdfDocumentEvent)currentEvent;
                    var pdfDoc = docEvent.GetDocument();
                    var page = docEvent.GetPage();
                    var pdfPageSize = page.GetPageSize();

                    var canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                    var document = new Document(pdfDoc);

                    var image = new Image(ImageDataFactory.Create(_imagePath));
                    image.SetFixedPosition(10, pdfPageSize.GetTop() - 120) // Adjusted position
                        .SetWidth(100) // Adjusted width
                        .SetHeight(100); // Adjusted height

                    document.Add(image);
                }
            }

        }
    }
    public static class DynamicExtensions
    {
        public static object GetProperty(dynamic obj, string propertyName)
        {
            if (obj is ExpandoObject)
            {
                var dictionary = (IDictionary<string, object>)obj;
                dictionary.TryGetValue(propertyName, out var value);
                return value;
            }
            else if (obj is IDictionary<string, object> dictionary)
            {
                dictionary.TryGetValue(propertyName, out var value);
                return value;
            }

            return null;
        }

        public static IEnumerable<object> GetEnumerable(dynamic obj)
        {
            if (obj is IEnumerable<object> enumerable)
            {
                return enumerable;
            }
            else if (obj is IEnumerable<object> list)
            {
                return list;
            }

            return Enumerable.Empty<object>();
        }
    }
}
