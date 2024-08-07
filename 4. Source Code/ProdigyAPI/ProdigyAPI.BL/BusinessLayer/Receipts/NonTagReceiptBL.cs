using Newtonsoft.Json;
using ProdigyAPI.BL.BusinessLayer.APIHandler;
using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Receipts;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Receipts
{
    public class NonTagReceiptBL
    {
        MagnaDbEntities db = null;

        public NonTagReceiptBL()
        {
            db = new MagnaDbEntities(true);
        }

        public NonTagReceiptBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }        

        //Note: Used BarcodeReceiptBL class to handle both barcode receipts and non-tag receipts

    }
}
