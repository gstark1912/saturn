using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelLibrary.DataAnnotations;
using ExcelLibrary.Exceptions;
using ExcelLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using X15 = DocumentFormat.OpenXml.Office2013.Excel;

namespace ExcelLibrary
{
    public class ExcelService : IExcelService
    {
        public void WriteDataInMemory<T>( MemoryStream stream, IEnumerable<T> data, bool writeHeader ) where T : class
        {
            using( SpreadsheetDocument document = SpreadsheetDocument.Create( stream, SpreadsheetDocumentType.Workbook ) )
            {
                GenerateNewExcel( document, data, writeHeader );
            }
        }

        public void WriteDataInFile<T>( string path, IEnumerable<T> data, bool writeHeader ) where T : class
        {
            try
            {
                if( File.Exists( path ) )
                    AddDataToExistingFile( path, data, false );
                else
                    CreateNewExcel( path, data, writeHeader );
            }
            catch( Exception )
            {
                throw new CAMSExcelLibraryException( "Hubo un problema en la escritura del Excel. Aseguresé que la carpeta de destino es accesible y el archivo de Excel no esté abierto.\nEl archivo es: " + path );
            }
        }

        private void CreateNewExcel<T>( string path, IEnumerable<T> data, bool writeHeader ) where T : class
        {
            CreateNeededFolders( path );
            using( SpreadsheetDocument document = SpreadsheetDocument.Create( path, SpreadsheetDocumentType.Workbook ) )
            {
                GenerateNewExcel( document, data, writeHeader );
            }
        }

        private void GenerateNewExcel<T>( SpreadsheetDocument document, IEnumerable<T> data, bool writeHeader ) where T : class
        {
            Columns columns = GenerateColumns( typeof( T ) );
            SheetData partSheetData = GenerateSheetdataForDetails( data, writeHeader, new SheetData() );

            WorkbookPart workbookPart1 = document.AddWorkbookPart();
            GenerateWorkbookPartContent( workbookPart1 );

            WorkbookStylesPart workbookStylesPart1 = workbookPart1.AddNewPart<WorkbookStylesPart>( "rId3" );
            GenerateWorkbookStylesPartContent( workbookStylesPart1 );

            WorksheetPart worksheetPart1 = workbookPart1.AddNewPart<WorksheetPart>( "rId1" );
            GenerateWorksheetPartContent( worksheetPart1, partSheetData, columns );
        }

        private void CreateNeededFolders( string path )
        {
            var folders = path.Split( '\\' );
            string folder = "";
            foreach( var f in folders.Take( folders.Count() - 1 ) )
            {
                folder += f;
                if( !Directory.Exists( folder ) )
                    Directory.CreateDirectory( folder );
                folder += "\\";
            }
        }

        private void AddDataToExistingFile<T>( string path, IEnumerable<T> data, bool v ) where T : class
        {
            using( SpreadsheetDocument document = SpreadsheetDocument.Open( path, true ) )
            {
                WorkbookPart workbookPart = document.WorkbookPart;
                IEnumerable<DocumentFormat.OpenXml.Spreadsheet.Sheet> sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById( relationshipId );
                SheetData sheetdata = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                SheetData partSheetData = GenerateSheetdataForDetails( data, false, sheetdata );

                worksheetPart.Worksheet.Save();
                document.Save();
            }
        }

        public List<T> GetRowsDataFromFile<T>( string filePath, bool fileHasHeader = false ) where T : new()
        {
            try
            {
                List<T> result = new List<T>();
                //open the excel using openxml sdk
                using( SpreadsheetDocument doc = SpreadsheetDocument.Open( filePath, false ) )
                {
                    //create the object for workbook part
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();

                    //using for each loop to get the sheet from the sheetcollection
                    Sheet thesheet = (Sheet)thesheetcollection.First();

                    //statement to get the worksheet object by using the sheet id
                    Worksheet theWorksheet = ( (WorksheetPart)workbookPart.GetPartById( thesheet.Id ) ).Worksheet;

                    SheetData thesheetdata = (SheetData)theWorksheet.GetFirstChild<SheetData>();
                    foreach( Row thecurrentrow in thesheetdata )
                    {
                        if( !fileHasHeader )
                        {
                            bool valueObtained = false;
                            int cont = 0;
                            T entity = new T();
                            PropertyInfo[] propertyInfo = entity.GetType().GetProperties();
                            foreach( Cell thecurrentcell in thecurrentrow )
                            {
                                if( propertyInfo.Count() > cont )
                                {
                                    if( SetValueToProperty( entity, propertyInfo[cont], thecurrentcell, workbookPart ) )
                                        valueObtained = true;
                                    cont++;
                                }
                            }

                            // If no value was obtained, then no entity will be added to the list as this could be an empty row
                            if( valueObtained )
                                result.Add( entity );
                        }
                        else
                            fileHasHeader = false;
                    }
                }

                return result;
            }
            catch( Exception )
            {
                throw new CAMSExcelLibraryException( "Hubo un problema al leer el Excel de ingreso. Aseguresé que la carpeta es accesible y el archivo de Excel no esté abierto.\nEl archivo es: " + filePath );
            }
        }

        private bool SetValueToProperty<T>( T entity, PropertyInfo propertyInfo, Cell thecurrentcell, WorkbookPart workbookPart ) where T : new()
        {
            //statement to take the integer value
            string currentcellvalue = string.Empty;
            if( thecurrentcell.DataType != null && thecurrentcell.DataType == CellValues.SharedString )
            {
                int id;
                if( Int32.TryParse( thecurrentcell.InnerText, out id ) )
                {
                    SharedStringItem item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt( id );
                    if( item.Text != null )
                    {
                        //code to take the string value
                        currentcellvalue = item.Text.Text + " ";
                    }
                    else if( item.InnerText != null )
                    {
                        currentcellvalue = item.InnerText;
                    }
                    else if( item.InnerXml != null )
                    {
                        currentcellvalue = item.InnerXml;
                    }
                }
            }
            else
            {
                currentcellvalue = thecurrentcell.InnerText + " ";
            }

            if( !String.IsNullOrEmpty( currentcellvalue ) )
            {
                // Integer type
                if( propertyInfo.PropertyType.Equals( typeof( int ) ) )
                {
                    int id;
                    if( Int32.TryParse( currentcellvalue, out id ) )
                        propertyInfo.SetValue( entity, id );
                    else
                        return false;
                }
                else if( propertyInfo.PropertyType.Equals( typeof( Int64 ) ) )
                {
                    Int64 id;
                    if( Int64.TryParse( currentcellvalue, out id ) )
                        propertyInfo.SetValue( entity, id );
                    else
                        return false;
                }
                // String type
                else
                {
                    var removeBlanks = propertyInfo.GetCustomAttribute<RemoveBlanksAttribute>();
                    if( removeBlanks != null )
                        currentcellvalue = currentcellvalue.Replace( " ", "" );
                    propertyInfo.SetValue( entity, currentcellvalue );
                }
                return true;
            }
            return false;
        }

        #region Excel Items Genweration

        private void GenerateWorkbookPartContent( WorkbookPart workbookPart1 )
        {
            Workbook workbook1 = new Workbook();
            Sheets sheets1 = new Sheets();
            Sheet sheet1 = new Sheet() { Name = "Sheet1", SheetId = (UInt32Value)1U, Id = "rId1" };
            sheets1.Append( sheet1 );
            workbook1.Append( sheets1 );
            workbookPart1.Workbook = workbook1;
        }

        private void GenerateWorksheetPartContent( WorksheetPart worksheetPart1, SheetData sheetData1, Columns columns )
        {
            Worksheet worksheet1 = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            worksheet1.AddNamespaceDeclaration( "r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships" );
            worksheet1.AddNamespaceDeclaration( "mc", "http://schemas.openxmlformats.org/markup-compatibility/2006" );
            worksheet1.AddNamespaceDeclaration( "x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac" );
            SheetDimension sheetDimension1 = new SheetDimension() { Reference = "A1" };

            SheetViews sheetViews1 = new SheetViews();

            SheetView sheetView1 = new SheetView() { TabSelected = true, WorkbookViewId = (UInt32Value)0U };
            Selection selection1 = new Selection() { ActiveCell = "A1", SequenceOfReferences = new ListValue<StringValue>() { InnerText = "A1" } };

            sheetView1.Append( selection1 );

            sheetViews1.Append( sheetView1 );
            SheetFormatProperties sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, DyDescent = 0.25D };

            columns.Append( new Column() { Min = 1, Max = 3, Width = 20, CustomWidth = true } );
            columns.Append( new Column() { Min = 4, Max = 4, Width = 30, CustomWidth = true } );

            PageMargins pageMargins1 = new PageMargins() { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };
            worksheet1.Append( sheetDimension1 );
            worksheet1.Append( sheetViews1 );
            worksheet1.Append( sheetFormatProperties1 );
            worksheet1.Append( columns );
            worksheet1.Append( sheetData1 );
            worksheet1.Append( pageMargins1 );
            worksheetPart1.Worksheet = worksheet1;
        }

        private void GenerateWorkbookStylesPartContent( WorkbookStylesPart workbookStylesPart1 )
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration( "mc", "http://schemas.openxmlformats.org/markup-compatibility/2006" );
            stylesheet1.AddNamespaceDeclaration( "x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac" );

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)2U, KnownFonts = true };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            Color color1 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append( fontSize1 );
            font1.Append( color1 );
            font1.Append( fontName1 );
            font1.Append( fontFamilyNumbering1 );
            font1.Append( fontScheme1 );

            Font font2 = new Font();
            Bold bold1 = new Bold();
            FontSize fontSize2 = new FontSize() { Val = 11D };
            Color color2 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName2 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };

            font2.Append( bold1 );
            font2.Append( fontSize2 );
            font2.Append( color2 );
            font2.Append( fontName2 );
            font2.Append( fontFamilyNumbering2 );
            font2.Append( fontScheme2 );

            fonts1.Append( font1 );
            fonts1.Append( font2 );

            Fills fills1 = new Fills() { Count = (UInt32Value)2U };

            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };

            fill1.Append( patternFill1 );

            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

            fill2.Append( patternFill2 );

            fills1.Append( fill1 );
            fills1.Append( fill2 );

            Borders borders1 = new Borders() { Count = (UInt32Value)2U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append( leftBorder1 );
            border1.Append( rightBorder1 );
            border1.Append( topBorder1 );
            border1.Append( bottomBorder1 );
            border1.Append( diagonalBorder1 );

            Border border2 = new Border();

            LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color color3 = new Color() { Indexed = (UInt32Value)64U };

            leftBorder2.Append( color3 );

            RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color color4 = new Color() { Indexed = (UInt32Value)64U };

            rightBorder2.Append( color4 );

            TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color color5 = new Color() { Indexed = (UInt32Value)64U };

            topBorder2.Append( color5 );

            BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color color6 = new Color() { Indexed = (UInt32Value)64U };

            bottomBorder2.Append( color6 );
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            border2.Append( leftBorder2 );
            border2.Append( rightBorder2 );
            border2.Append( topBorder2 );
            border2.Append( bottomBorder2 );
            border2.Append( diagonalBorder2 );

            borders1.Append( border1 );
            borders1.Append( border2 );

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append( cellFormat1 );

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)3U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyBorder = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = true };

            cellFormats1.Append( cellFormat2 );
            cellFormats1.Append( cellFormat3 );
            cellFormats1.Append( cellFormat4 );

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append( cellStyle1 );
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration( "x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main" );
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append( slicerStyles1 );

            StylesheetExtension stylesheetExtension2 = new StylesheetExtension() { Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
            stylesheetExtension2.AddNamespaceDeclaration( "x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main" );
            X15.TimelineStyles timelineStyles1 = new X15.TimelineStyles() { DefaultTimelineStyle = "TimeSlicerStyleLight1" };

            stylesheetExtension2.Append( timelineStyles1 );

            stylesheetExtensionList1.Append( stylesheetExtension1 );
            stylesheetExtensionList1.Append( stylesheetExtension2 );

            stylesheet1.Append( fonts1 );
            stylesheet1.Append( fills1 );
            stylesheet1.Append( borders1 );
            stylesheet1.Append( cellStyleFormats1 );
            stylesheet1.Append( cellFormats1 );
            stylesheet1.Append( cellStyles1 );
            stylesheet1.Append( differentialFormats1 );
            stylesheet1.Append( tableStyles1 );
            stylesheet1.Append( stylesheetExtensionList1 );

            workbookStylesPart1.Stylesheet = stylesheet1;
        }

        #endregion

        #region Columns and header

        private Columns GenerateColumns( Type type )
        {
            Columns workColumn = new Columns();
            PropertyInfo[] propertyInfo = type.GetProperties();
            UInt32Value i = 1U;
            foreach( var prop in propertyInfo )
            {
                var width = prop.GetCustomAttribute<WidthAttribute>();
                workColumn.Append( new Column() { Min = i, Max = i, Width = width != null ? width.Width : 20, CustomWidth = true } );
                i++;
            }
            return workColumn;
        }

        private Row CreateHeaderRowForExcel( Type type )
        {
            Row workRow = new Row();
            PropertyInfo[] propertyInfo = type.GetProperties();
            foreach( var prop in propertyInfo )
            {
                var header = prop.GetCustomAttribute<HeaderAttribute>();
                workRow.Append( CreateCell( header != null ? header.Text : prop.Name, 2U, true ) );
            }
            return workRow;
        }

        #endregion

        #region Data insert

        private SheetData GenerateSheetdataForDetails<T>( IEnumerable<T> data, bool writeHeader, SheetData sheetData1 ) where T : class
        {
            if( writeHeader )
                sheetData1.Append( CreateHeaderRowForExcel( typeof( T ) ) );

            foreach( var obj in data )
            {
                Row partsRows = GenerateRowForChildPartDetail( obj );
                sheetData1.Append( partsRows );
            }
            return sheetData1;
        }

        private Row GenerateRowForChildPartDetail<T>( T testmodel ) where T : class
        {
            Row tRow = new Row();
            PropertyInfo[] propsInfo = typeof( T ).GetProperties();
            foreach( var property in propsInfo )
            {
                var prop = property.GetCustomAttribute<StringDatatypeAttribute>();
                tRow.Append( CreateCell( property.GetValue( testmodel ) != null ? property.GetValue( testmodel ).ToString() : "", forceString: prop != null ) );
            }

            return tRow;
        }

        private Cell CreateCell( string text, uint styleIndex = 1U, bool forceString = false )
        {
            Cell cell = new Cell();
            cell.StyleIndex = styleIndex;
            cell.DataType = forceString ? (EnumValue<CellValues>)CellValues.InlineString : ResolveCellDataTypeOnValue( text );
            if( cell.DataType == CellValues.InlineString )
                cell.InlineString = new InlineString() { Text = new Text( text ) };
            else
                cell.CellValue = new CellValue( text );
            return cell;
        }

        private EnumValue<CellValues> ResolveCellDataTypeOnValue( string text )
        {
            int intVal;
            double doubleVal;
            if( int.TryParse( text, out intVal ) || double.TryParse( text, out doubleVal ) )
            {
                return CellValues.Number;
            }
            else
            {
                return CellValues.InlineString;
            }
        }

        #endregion
    }
}