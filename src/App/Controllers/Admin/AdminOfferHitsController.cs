using App.DTO;
using App.Extensions;
using Model.Context;
using Model.Model.Customer;
using Model.Ranking;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace App.Controllers.Admin
{
    public class AdminOfferHitsController : Controller
    {
        public ModelContext modelContext;

        public AdminOfferHitsController()
        {
            this.modelContext = new ModelContext();
        }
        // GET: AdminOfferHits
        public ActionResult Index( string FromDate = null, string ToDate = null, string message = "" )
        {
            ViewBag.Message = message;
            if( FromDate != null && ToDate != null )
            {
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.TopHits = LoadStats( ViewBag.FromDate.ToString(), ViewBag.ToDate.ToString() );
            }
            else
            {
                var lastMonth = DateTime.Now.AddMonths( -1 );
                ViewBag.FromDate = String.Format( "{2}-{1}-{0}", "01", lastMonth.Month.ToString( "00" ), lastMonth.Year );
                ViewBag.ToDate = String.Format( "{2}-{1}-{0}", DateTime.DaysInMonth( lastMonth.Year, lastMonth.Month ), lastMonth.Month.ToString( "00" ), lastMonth.Year );
                ViewBag.TopHits = new List<OfferHitsDTO>();
                ViewBag.MailRecords = GetMailRecords();
            }

            return View( "~/Views/AdminOfferHits/Index.cshtml" );
        }

        // GET: AdminOfferHits
        public ActionResult SendMails( string FromDate = null, string ToDate = null )
        {
            if( FromDate != null && ToDate != null )
            {
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                var stats = LoadStats( ViewBag.FromDate.ToString(), ViewBag.ToDate.ToString() );
                ViewBag.CompaniesStats = GetCompaniesStats( stats );
                ViewBag.MailRecords = GetMailRecords();
            }
            else
            {
                return RedirectToAction( "Index" );
            }

            return View( "~/Views/AdminOfferHits/SendMails.cshtml" );
        }

        private List<MailRecord> GetMailRecords()
        {
            return this.modelContext.MailRecords.OrderByDescending( x => x.TimeStamp ).ToList();
        }

        private string GetPeriod( string fromDate, string toDate )
        {
            int month = 0;
            var from = DateTime.Parse( fromDate );
            var to = DateTime.Parse( toDate );

            if( from.Month == to.Month && // mismo mes
                from.Day == 1 && // primer dia del mes
                to.Day == DateTime.DaysInMonth( to.Year, to.Month ) ) // ultimo dia del mes
                month = from.Month;

            if( month != 0 )
                return String.Format( "el mes de {0}", GetMonth( month ) );
            else
                return String.Format( "el período {0}~{1}", from.ToShortDateString(), to.ToShortDateString() );
        }

        // GET: AdminOfferHits
        public ActionResult ConfirmEmails( string FromDate = null, string ToDate = null )
        {
            if( FromDate != null && ToDate != null )
            {
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                var period = GetPeriod( ViewBag.FromDate.ToString(), ViewBag.ToDate.ToString() );
                List<OfferHitsDTO> stats = LoadStats( ViewBag.FromDate.ToString(), ViewBag.ToDate.ToString() );
                var companies = GetCompaniesStats( stats );
                SendMailsToCompanies( stats, period );
                SaveMailRecord( FromDate, ToDate );

                return RedirectToAction( "Index", new { message = String.Format( "Se han enviado {0} mails", stats.Count ) } );
            }
            else
            {
                return RedirectToAction( "Index" );
            }
        }

        private void SaveMailRecord( string fromDate, string toDate )
        {
            var from = DateTime.Parse( fromDate );
            var to = DateTime.Parse( toDate );

            MailRecord newEntry = new MailRecord();
            newEntry.FromDate = from;
            newEntry.ToDate = to;

            this.modelContext.MailRecords.Add( newEntry );
            this.modelContext.SaveChanges();
        }

        private string GetMonth( int month )
        {
            string[] months = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            return months[month - 1];
        }

        private void SendMailsToCompanies( List<OfferHitsDTO> products, string period )
        {
            var debugMode = ConfigurationManager.AppSettings["DebugMode"] != null ? ConfigurationManager.AppSettings["DebugMode"].ToString() == "1" : false;

            var companyIds = products.Select( x => x.Offer.CompanyId );
            var aux = this.modelContext
                .Companies
                .Include( "ComercialContact" )
                .Where( x => companyIds.Contains( x.Id ) ).ToList();

            string htmlTopTenTable = GetTop10HtmlTable( products );

            foreach( var c in products )
            {
                var contact = aux.FirstOrDefault( x => x.Id == c.Offer.CompanyId ).ComercialContact;
                var emailSender = new EmailSender();

                var email = ConfigurationManager.AppSettings["SMTPUsername"];
                var subject = String.Format( "{0}, ha sido recomendado {1} {2} en {3}", c.Offer.ProductName, c.Count, c.Count == 1 ? "vez" : "veces", period );
                string content = System.IO.File.ReadAllText( Server.MapPath( Url.Content( "~/Utils/StatsEmail.html" ) ) );
                var body =
                    content
                    .Replace( "{ContactFullName}", contact.FullName )
                    .Replace( "{Count}", c.Count.ToString() )
                    .Replace( "{Times}", c.Count == 1 ? "vez" : "veces" )
                    .Replace( "{ProductName}", c.Offer.ProductName )
                    .Replace( "{Period}", period );

                //Tabla Top 10 oculta del mail por el momento
                //Replace( "{HTMLTable}", htmlTopTenTable )

                string emailTo;
                if( debugMode )
                    emailTo = "probandosaturn@yopmail.com";
                else
                    emailTo = contact.Email;

                MailMessage mailMessage = new MailMessage( email, emailTo, subject, body )
                {
                    IsBodyHtml = true
                };

                emailSender.Send( mailMessage );
                if( debugMode )
                    break;
            }
        }

        private string GetTop10HtmlTable( List<OfferHitsDTO> products )
        {
            string response = "";
            var top = products.OrderByDescending( x => x.Count ).Take( 10 );
            foreach( var p in top )
            {
                response += String.Format( "<tr><td style=\"width: 50.0000 %; \"><span style=\"font - family: Verdana, Geneva, sans - serif; font - size: 13px; \">{0}</span></td>", p.Offer.ProductName );
                response += String.Format( "<td style=\"width: 50.0000 %; \"><span style=\"font - family: Verdana, Geneva, sans - serif; font - size: 13px; \">{0}</span></td></tr>", p.Count );
            }

            return response;
        }

        private List<CompanyHitsDTO> GetCompaniesStats( List<OfferHitsDTO> stats )
        {
            List<int> ids = stats.Select( x => x.Offer.CompanyId ).Distinct().ToList();
            List<CompanyHitsDTO> response = new List<CompanyHitsDTO>();
            foreach( var id in ids )
            {
                var aux = new CompanyHitsDTO { CompanyId = id, CompanyName = stats.FirstOrDefault( x => x.Offer.CompanyId == id ).Offer.CompanyName };
                aux.Offer.AddRange( stats.Where( x => x.Offer.CompanyId == id ).Select( x => new OfferHitsDTO { Offer = x.Offer, Count = x.Count } ) );
                response.Add( aux );
            }

            return response;
        }

        private List<OfferHitsDTO> LoadStats( string fromDate, string toDate )
        {
            var from = DateTime.Parse( fromDate );
            var to = DateTime.Parse( toDate );

            List<int> query = this.modelContext
                .SurveyCompletionParent
                .Where( x =>
                     x.Customer.Role == "DEMANDA" &&
                     !x.PartialSave &&
                     x.CreatedAt >= from &&
                     x.CreatedAt < to )
                     .Select( x => x.Id )
                     .ToList();

            List<Ranking> completeRanking = new List<Ranking>();
            foreach( var id in query )
            {
                List<Ranking> rank = this.modelContext
                  .Rankings
                  .Include( "SurveyCompletionParentByOferta" )
                  .Include( "SurveyCompletionParentByOferta.Product" )
                  .Include( "SurveyCompletionParentByOferta.Product.Company" )
                  .Where( x => x.SurveyCompletionParentByDemanda.Id == id )
                  .OrderByDescending( x => x.Rank )
                  .ToList();

                completeRanking.AddRange( rank.Take( 10 ) );
            }

            var aux = completeRanking
                .GroupBy( x => new
                {
                    ProductId = x.SurveyCompletionParentByOferta.Product.Id,
                    ProductName = x.SurveyCompletionParentByOferta.Product.Name,
                    CompanyId = x.SurveyCompletionParentByOferta.Product.CompanyId,
                    CompanyName = x.SurveyCompletionParentByOferta.Product.Company.CompanyName
                } )
                .Select( y => new OfferHitsDTO
                {
                    Offer = MapToOffer( y.Key ),
                    Count = y.Count()
                } )
                .OrderByDescending( x => x.Count )
                .ToList();

            return aux;
        }

        private OfferDTO MapToOffer( object key )
        {
            return (OfferDTO)key.ToType<OfferDTO>( new OfferDTO() );
        }
    }
}