using System;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Colors;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using Image = iText.Layout.Element.Image;
using iText.Kernel.Pdf.Canvas.Draw;

class Report
{
    
    public static void GenerateMedicalReport(string name, string age, string gender, string mainImagePath, string referredDoctorImagePath
    ,string doctorFindings,string doctorRecommendation,string folderpath)
    {
        // string reportFilePath = "D://newtest/MedicalReport.pdf";
        string reportFilePath = $"{folderpath}/MedicalReport.pdf";
        string logoImagePath = "D://newtest/Score.png";

        try
        {
            using (PdfWriter writer = new PdfWriter(reportFilePath))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);
                    document.SetMargins(20, 20, 20, 20);
                        // this doesnt work 
                    iText.Kernel.Geom.PageSize pageSize = pdf.GetDefaultPageSize();
                    SolidBorder solidBorder = new SolidBorder(ColorConstants.BLACK, 2);
                    pdf.AddNewPage();
                    pdf.GetPage(1).SetMediaBox(new iText.Kernel.Geom.Rectangle((int)pageSize.GetLeft(), (int)pageSize.GetBottom(), (int)pageSize.GetWidth(), (int)pageSize.GetHeight()));
                    pdf.GetPage(1).SetCropBox(new iText.Kernel.Geom.Rectangle((int)pageSize.GetLeft(), (int)pageSize.GetBottom(), (int)pageSize.GetWidth(), (int)pageSize.GetHeight()));

                    if (File.Exists(logoImagePath))
                    {
                        ImageData logoData = ImageDataFactory.Create(logoImagePath);
                        Image logo = new Image(logoData).ScaleToFit(100, 100).SetFixedPosition(20, pageSize.GetTop() - 95);
                        document.Add(logo);
                    }
                    Paragraph title = new Paragraph("Medical Report")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20)
                        .SetBold();
                    document.Add(title);

                    Paragraph subtitle = new Paragraph("Score Medical Center")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(16)
                        // .SetBold()
                        .SetMarginLeft(5)
                        .SetMarginTop(35);
                    document.Add(subtitle);

                    document.Add(new LineSeparator(new SolidLine()).SetMarginTop(15));

                    Paragraph patientInfo = new Paragraph()
                        .Add(new Text("Patient Information\n").SetBold().SetFontSize(14))
                        .Add("Name: " + name + "\n")
                        .Add("Age: " + age + "\n")
                        .Add("Gender: " + gender + "\n")
                        .SetMarginTop(50);
                    document.Add(patientInfo);

                    if (File.Exists(mainImagePath))
                    {
                        ImageData mainImageData = ImageDataFactory.Create(mainImagePath);
                        Image mainImage = new Image(mainImageData)
                            .ScaleToFit(400, 400)
                            .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                            .SetMarginTop(10);
                        document.Add(mainImage);
                    }

                    Paragraph findingsBox = new Paragraph()
                        .Add(new Text("Doctor's Findings\n").SetBold().SetFontSize(14))
                        // .Add("The X-ray shows a clear chest with no signs of infection or abnormal growth. The heart and lungs are of normal size and shape. No signs of fluid build-up or other issues.")
                        .Add(doctorFindings)
                        .SetBorder(new SolidBorder(1))
                        .SetPadding(10)
                        .SetMarginTop(20);
                    document.Add(findingsBox);

                    Paragraph recommendationsBox = new Paragraph()
                        .Add(new Text("Doctor's Recommendations\n").SetBold().SetFontSize(14))
                        // .Add("Regular check-ups are advised to monitor the condition. Maintain a healthy diet and regular exercise to support overall health. Follow up with your primary care physician for any new symptoms.")
                        .Add(doctorRecommendation)
                        .SetBorder(new SolidBorder(1))
                        .SetPadding(10)
                        .SetMarginTop(20);
                    document.Add(recommendationsBox);

                    document.Add(new LineSeparator(new SolidLine()).SetMarginTop(20));

                    Paragraph date = new Paragraph("Date: " + DateTime.Now.ToString("MM/dd/yyyy"))
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12)
                        .SetMarginTop(10);
                    document.Add(date);

                    Paragraph doctorName = new Paragraph("Doctor: Dr. Noore Al-sadon")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12)
                        .SetMarginTop(5);
                    document.Add(doctorName);
                    if (File.Exists(referredDoctorImagePath))
                    {
                        ImageData referredDoctorImageData = ImageDataFactory.Create(referredDoctorImagePath);
                        Image referredDoctorImage = new Image(referredDoctorImageData)
                            .ScaleToFit(100, 100)
                            .SetHorizontalAlignment(HorizontalAlignment.LEFT)
                            .SetMarginTop(10);
                        document.Add(referredDoctorImage);
                    }
                    Paragraph referredDoctorName = new Paragraph("Referred Doctor: Dr. Nour mansour")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12)
                        .SetMarginTop(8);
                    document.Add(referredDoctorName);



                    document.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while generating the medical report: " + ex.Message);
        }


    }
}
