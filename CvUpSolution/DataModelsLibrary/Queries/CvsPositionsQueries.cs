﻿using Database.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public class CvsPositionsQueries: ICvsPositionsQueries
    {
        private cvup00001Context dbContext;

        public CvsPositionsQueries()
        {
            dbContext = new cvup00001Context();
        }

    public void AddCv()
        {

        }

        public int GetUniqueCvId()
        {
            var ci = new cvs_incremental
            {
                name="cc"
            };

            dbContext.cvs_incrementals.Add(ci);
            dbContext.SaveChanges();
            return ci.id;
        }

        public candidate AddNewCandidate(string email, string phone)
        {
            var cand = new candidate
            {
                email = email,
                phone = phone
            };

            dbContext.candidates.Add(cand);
            dbContext.SaveChanges();
            return cand;
        }

        public candidate? GetCandidateByEmail(string email)
        {
            candidate? cand = dbContext.candidates.Where(x => x.email == email).FirstOrDefault();
            return cand;
        }

    }
}