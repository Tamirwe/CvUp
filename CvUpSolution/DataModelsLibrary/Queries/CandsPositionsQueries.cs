﻿using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using GeneralLibrary;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataModelsLibrary.Queries
{
    public partial class CandsPositionsQueries : ICandsPositionsQueries
    {

        public CandsPositionsQueries()
        {
        }

        public async Task<CandModel?> GetCandidate(int companyId, int candId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where cand.company_id == companyId && cand.id == candId
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review,
                                 reviewDate = cand.review_date,
                                 allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 city = cand.city,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = Convert.ToDateTime(cand.last_cv_sent),
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                 isSeen = Convert.ToBoolean(cvs.is_seen)
                             }).Take(300);

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
        }

        public async Task<CandModel?> GetPositionCandidate(int companyId, int candId, int positionId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join pcv in dbContext.position_candidates on cand.id equals pcv.candidate_id
                             join cvs in dbContext.cvs on pcv.cv_id equals cvs.id
                             where cand.company_id == companyId && cand.id == candId && pcv.position_id == positionId
                             select new CandModel
                             {
                                cvId = cvs.id,
                                review = cand.review,
                                reviewDate = cand.review_date,
                                allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                keyId = cvs.key_id,
                                candidateId = cand.id,
                                email = cand.email,
                                emailSubject = cvs.subject,
                                firstName = cand.first_name,
                                lastName = cand.last_name,
                                phone = cand.phone,
                                city = cand.city,
                                hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                cvSent = Convert.ToDateTime(cand.last_cv_sent),
                                candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                isSeen = Convert.ToBoolean(cvs.is_seen),
                                candPosHistory = new CandPosHistoryModel
                                {
                                    accepted = pcv.accepted,
                                    callEmailToCandidate = pcv.call_email_to_candidate,
                                    customerInterview = pcv.customer_interview,
                                    emailToContact = pcv.email_to_contact,
                                    rejected = pcv.rejected,
                                    rejectEmailToCandidate = pcv.reject_email_to_candidate,
                                    removeCandidacy = pcv.remove_candidacy,
                                }
                             }).Take(300);

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
        }

        public async Task<List<CandModel?>> GetCandsList(int companyId,  List<int>? candsIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where cand.company_id == companyId && candsIds != null ? candsIds.Contains(cand.id) : 1 == 1
                             orderby cand.last_cv_sent descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review,
                                 reviewDate = cand.review_date,
                                 allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 city = cand.city,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = Convert.ToDateTime(cand.last_cv_sent),
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                 isSeen = Convert.ToBoolean(cvs.is_seen)
                             }).Take(300);

                var result = await query.ToListAsync();
                return result;
            }
        }

        public async Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join pcv in dbContext.position_candidates on cand.id equals pcv.candidate_id
                             join cvs in dbContext.cvs on pcv.cv_id equals cvs.id
                             where pcv.company_id == companyId
                                    && pcv.position_id == positionId
                             orderby pcv.date_updated descending
                             select new CandModel
                             {
                                 cvId = pcv.cv_id,
                                 posCvId = pcv.cv_id,
                                 customerReview = pcv.customer_review,
                                 allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                 review = cand.review,
                                 reviewDate = cand.review_date,
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 city = cand.city,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = cvs.date_created,
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                 isSeen = Convert.ToBoolean(cvs.is_seen),
                                 candPosHistory = new CandPosHistoryModel
                                 {
                                     accepted = pcv.accepted,
                                     callEmailToCandidate = pcv.call_email_to_candidate,
                                     customerInterview = pcv.customer_interview,
                                     emailToContact = pcv.email_to_contact,
                                     rejected = pcv.rejected,
                                     rejectEmailToCandidate = pcv.reject_email_to_candidate,
                                     removeCandidacy = pcv.remove_candidacy,
                                 }
                             });

                return await query.ToListAsync();
            }
        }

        public async Task<List<CandModel?>> GetPosTypeCandsList(int companyId, int positionTypeId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where cand.company_id == companyId && cvs.position_type_id == positionTypeId
                             orderby cand.last_cv_sent descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review,
                                 reviewDate = cand.review_date,
                                 allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 city = cand.city,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = Convert.ToDateTime(cand.last_cv_sent),
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                 isSeen = Convert.ToBoolean(cvs.is_seen)
                             }).Take(300);

                var result = await query.ToListAsync();
                return result;
            }
        }

        public async Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join fc in dbContext.folders_cands on cand.id equals fc.candidate_id
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where fc.company_id == companyId
                                    && fc.folder_id == folderId
                             orderby cand.last_cv_sent descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review,
                                 reviewDate = cand.review_date,
                                 allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 city = cand.city,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = cvs.date_created,
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                 isSeen = Convert.ToBoolean(cvs.is_seen)
                             });

                return await query.ToListAsync();
            }
        }

        public async Task<int> AddCv(ImportCvModel importCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                var newCv = new cv
                {
                    company_id = Convert.ToInt32(importCv.companyId),
                    candidate_id = importCv.candidateId,
                    email_id = importCv.emailId,
                    subject = importCv.subject,
                    from = importCv.from,
                    //duplicate_cv_id = importCv.duplicateCvId,
                    position = Utils.Truncate(importCv.positionRelated, 250),
                    file_type = importCv.fileTypeKey,
                    date_created = importCv.dateCreated,
                    position_type_id = importCv.positionTypeId,

                };

                dbContext.cvs.Add(newCv);
                await dbContext.SaveChangesAsync();

                var cvTxt = new cvs_txt
                {
                    cv_id = newCv.id,
                    candidate_id = importCv.candidateId,
                    company_id = importCv.companyId,
                    cv_txt = importCv.cvTxt.Length > 7999 ? importCv.cvTxt.Substring(0, 7999) : importCv.cvTxt,
                    email_subject = importCv.subject,
                    ascii_sum = importCv.cvAsciiSum,
                };

                dbContext.cvs_txts.Add(cvTxt);

                await dbContext.SaveChangesAsync();

                return newCv.id;
            }
        }

        public async Task DeleteCv(int companyId, int candidateId, int cvId)
        {
            using (var dbContext = new cvup00001Context())
            {
                List<cv>? candCvs = await dbContext.cvs.Where(x => x.company_id == companyId && x.candidate_id == candidateId).ToListAsync();

                if (candCvs.Count > 1)
                {
                    cv? cvToDelete = candCvs.Where(x => x.id == cvId).FirstOrDefault();

                    if (cvToDelete != null)
                    {
                        var result = dbContext.cvs.Remove(cvToDelete);
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    await DeleteCandidate(companyId, candidateId);
                }
            }
        }

        public async Task<Tuple<cv?, bool>> GetCandLastCv(int companyId, int candidateId)
        {
            using (var dbContext = new cvup00001Context())
            {
                List<cv>? candCvs = await dbContext.cvs.Where(x => x.company_id == companyId && x.candidate_id == candidateId).OrderByDescending(x => x.date_created).ToListAsync();
                cv? lastCv = candCvs.OrderByDescending(x => x.date_created).FirstOrDefault();
                var isDuplicate = candCvs.Count > 1;
                return new Tuple<cv?, bool>(lastCv, isDuplicate);
            }
        }

        public async Task UpdateCandLastCv(int companyId, int candidateId, int cvId, bool isDuplicate, DateTime lastCvSent)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate cand = dbContext.candidates.Where(x => x.company_id == companyId && x.id == candidateId).First();
                cand.has_duplicates_cvs = (sbyte?)(isDuplicate ? 1 : 0);
                cand.last_cv_id = cvId;
                cand.last_cv_sent = lastCvSent;
                cand.date_updated = DateTime.Now;
                var result = dbContext.candidates.Update(cand);
                await dbContext.SaveChangesAsync();
            }
        }


        public async Task DeleteCandidate(int companyId, int candidateId)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? candTodelete = dbContext.candidates.Where(x => x.company_id == companyId && x.id == candidateId).FirstOrDefault();

                if (candTodelete != null)
                {
                    var result = dbContext.candidates.Remove(candTodelete);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateCvDate(int cvId)
        {
            using (var dbContext = new cvup00001Context())
            {
                cv cv = dbContext.cvs.Where(x => x.id == cvId).First();
                cv.date_created = DateTime.Now;
                var result = dbContext.cvs.Update(cv);
                await dbContext.SaveChangesAsync();
            }
        }


        public async Task UpdateCvKeyId(ImportCvModel importCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                cv? cv = dbContext.cvs.Where(x => x.id == importCv.cvId).First();
                cv.key_id = importCv.cvKey;
                var result = dbContext.cvs.Update(cv);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<int> AddCandidate(candidate newCand)
        {
            using (var dbContext = new cvup00001Context())
            {
                dbContext.candidates.Add(newCand);
                await dbContext.SaveChangesAsync();
                return newCand.id;
            }
        }

        public async Task UpdateCandidate(candidate cand)
        {
            using (var dbContext = new cvup00001Context())
            {
                var result = dbContext.candidates.Update(cand);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<candidate?> GetCandidateByEmail(string email)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = await dbContext.candidates.Where(x => x.email == email).FirstOrDefaultAsync();
                return cand;
            }
        }

        public async Task<candidate?> GetCandidateByPhone(string phone)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = await dbContext.candidates.Where(x => x.phone == phone).FirstOrDefaultAsync();
                return cand;
            }
        }

        public async Task<List<CvsToIndexModel>> GetCompanyCvsToIndex(int companyId, int candidateId = 0)
        {
            using (var dbContext = new cvup00001Context())
            {

                var query = from cand in dbContext.candidates
                            where cand.company_id == companyId && candidateId > 0 ? cand.id == candidateId : 1 == 1
                            select new CvsToIndexModel
                            {
                                candidateId = cand.id,
                                firstName = cand.first_name,
                                lastName = cand.last_name,
                                email = cand.email,
                                phone = cand.phone,
                                reviewText = cand.review,
                                lastCvSent = cand.last_cv_sent
                            };

                List<CvsToIndexModel> candsCvs = await query.ToListAsync();

                var cvsTxtQuery = from cv in dbContext.cvs_txts
                                  where cv.company_id == companyId && candidateId > 0 ? cv.candidate_id == candidateId : 1 == 1
                                  orderby cv.candidate_id ascending, cv.ascii_sum 
                                  select new CvTxtModel
                                  {
                                      id = cv.id,
                                      candidateId = cv.candidate_id ?? 0,
                                      cvTxt = cv.cv_txt,
                                      asciiSum = cv.ascii_sum,
                                  };

                List<CvTxtModel> cvsTxts = await cvsTxtQuery.ToListAsync();

                //List <cvs_txt> cvsTxts = await dbContext.cvs_txts.Where(x => x.company_id == companyId).ToListAsync();

                List<CvTxtModel> candsGroupCvsTxt = GroupCandsCvsTxts(cvsTxts);

                foreach (var item in candsCvs)
                {
                    var candCvsTxt = candsGroupCvsTxt.Find(x => x.candidateId == item.candidateId);

                    if (candCvsTxt != null)
                    {
                        item.cvsTxt = candCvsTxt.cvTxt;
                    }
                }



                //foreach (var item in candsCvs)
                //{
                //    var asciiGroup = cvsTxts.Where(x => x.candidateId == item.candidateId).GroupBy(x => x.asciiSum).Select(x => x.First()).ToList();

                //    StringBuilder sb = new StringBuilder();

                //    foreach (var group in asciiGroup)
                //    {
                //        sb.Append(group.cvTxt + " ");
                //    }

                //    item.cvsTxt = sb.ToString();
                //}

                return candsCvs;
            }
        }

        private List<CvTxtModel> GroupCandsCvsTxts(List<CvTxtModel> candCvsTxts)
        {

            int asciiSumCurrent = -1;
            int candIdCurrent = 0;
            StringBuilder? sb = null;

            List<CvTxtModel> candsGroupCvsTxt = new List<CvTxtModel>();

            foreach (var item in candCvsTxts)
            {
                if (candIdCurrent != item.candidateId)
                {
                    if (sb != null)
                    {
                        candsGroupCvsTxt.Add(new CvTxtModel { candidateId = candIdCurrent, cvTxt = sb.ToString() });
                    }

                    candIdCurrent = item.candidateId ?? 0;
                    asciiSumCurrent = item.asciiSum ?? -1;
                    sb = new StringBuilder(item.cvTxt);
                }
                else
                {
                    if (asciiSumCurrent != item.asciiSum && sb != null)
                    {
                        sb.Append(" " + item.cvTxt);
                    }

                    candIdCurrent = item.candidateId ?? 0;
                    asciiSumCurrent = item.asciiSum ?? -1;
                }
            }

            if (sb != null)
            {
                candsGroupCvsTxt.Add(new CvTxtModel { candidateId = candIdCurrent, cvTxt = sb.ToString() });
            }

            return candsGroupCvsTxt;
        }


        public async Task<List<CandCvModel>> GetCandCvsList(int companyId, int candidateId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cvs in dbContext.cvs
                             where cvs.company_id == companyId
                             && cvs.candidate_id == candidateId
                             orderby cvs.date_created descending
                             select new CandCvModel
                             {
                                 candidateId = cvs.candidate_id,
                                 cvId = cvs.id,
                                 cvSent = cvs.date_created,
                                 keyId = cvs.key_id,
                                 emailSubject = cvs.subject,
                             });

                return await query.ToListAsync();
            }
        }

        public async Task UpdateCvsAsciiSum(int companyId)
        {

            using (var dbContext = new cvup00001Context())
            {
                List<cvs_txt>? cvsTxtList = await dbContext.cvs_txts.Where(x => x.ascii_sum == null).ToListAsync();
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                foreach (var itemCv in cvsTxtList)
                {
                    int docAsciiSum = 0;

                    if (itemCv.cv_txt != null)
                    {
                        foreach (char c in itemCv.cv_txt)
                        {
                            try
                            {
                                docAsciiSum += Convert.ToInt32(c);
                            }
                            catch (Exception) { }
                        }
                    }

                    itemCv.ascii_sum = docAsciiSum;
                }

                dbContext.cvs_txts.UpdateRange(cvsTxtList);

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<int>> getPositionContactsIds(int companyId, int positionId)
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.position_contacts
                     .Where(p => p.company_id == companyId && p.position_id == positionId)
                     .Select(p => p.contact_id)
                     .ToListAsync();
            }
        }

        public async Task<PositionModel> GetPosition(int companyId, int positionId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var inter = await dbContext.position_interviewers
                          .Where(p => p.company_id == companyId && p.position_id == positionId)
                          .Select(p => p.user_id)
                          .ToArrayAsync();

                var conts = await getPositionContactsIds(companyId, positionId);

                var query = from p in dbContext.positions
                            where p.id == positionId && p.company_id == companyId
                            join c in dbContext.customers on p.customer_id equals c.id
                            orderby p.name
                            select new PositionModel
                            {
                                id = p.id,
                                name = p.name,
                                descr = p.descr,
                                requirements = p.requirements,
                                customerId = p.customer_id ?? 0,
                                customerName = c.name,
                                status = System.Enum.Parse<PositionStatusEnum>(p.status),
                                interviewersIds = inter.ToArray(),
                                contactsIds = conts,
                                emailsubjectAddon = p.customer_pos_num,
                                remarks = p.remarks,
                                matchEmailsubject = p.match_email_subject,
                                updated = p.date_updated,
                                created = p.date_created
                            };

                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                return await query.FirstAsync();
            }
        }

        public async Task<position> AddPosition(PositionModel data, int companyId, int userId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var ent = new position
                {
                    company_id = companyId,
                    name = data.name,
                    descr = data.descr,
                    requirements = data.requirements,
                    status = data.status.ToString(),
                    opener_id = userId,
                    updater_id = userId,
                    date_created = DateTime.Now,
                    date_updated = DateTime.Now,
                    customer_pos_num = data.emailsubjectAddon,
                    remarks = data.remarks,
                    match_email_subject = data.matchEmailsubject
                };

                if (data.customerId > 0)
                {
                    ent.customer_id = data.customerId;
                }

                var result = dbContext.positions.Add(ent);
                await dbContext.SaveChangesAsync();

                return result.Entity;
            }
        }

        public async Task<position> UpdatePosition(PositionModel data, int companyId, int userId)
        {

            using (var dbContext = new cvup00001Context())
            {
                position? pos = dbContext.positions.Where(x => x.company_id == companyId && x.id == data.id).First();

                pos.name = data.name;
                pos.descr = data.descr;
                pos.requirements = data.requirements;
                pos.customer_id = data.customerId == 0 ? null : data.customerId;
                pos.status = data.status.ToString();
                pos.updater_id = userId;
                pos.date_updated = DateTime.Now;
                pos.customer_pos_num = data.emailsubjectAddon;
                pos.remarks = data.remarks;
                pos.match_email_subject = data.matchEmailsubject;

                var result = dbContext.positions.Update(pos);
                await dbContext.SaveChangesAsync();

                return result.Entity;
            }
        }

        public async Task<List<PositionModel>> GetPositionsList(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from p in dbContext.positions
                            join c in dbContext.customers on p.customer_id equals c.id
                            where p.company_id == companyId
                            orderby p.date_updated descending
                            select new PositionModel
                            {
                                id = p.id,
                                name = p.name,
                                status = System.Enum.Parse<PositionStatusEnum>(p.status),
                                updated = p.date_updated,
                                created = p.date_created,
                                customerName = c.name,
                                customerId = p.customer_id,
                                candsCount = p.cands_count
                            };

                return await query.ToListAsync();
            }
        }

        public async Task DeletePosition(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var ent = (from p in dbContext.positions
                           where p.id == id && p.company_id == companyId
                           select p).FirstOrDefault();

                if (ent != null)
                {
                    var result = dbContext.positions.Remove(ent);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task AddUpdateInterviewers(int companyId, int positionId, int[] interviewersIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dbInterviewer = await (from i in dbContext.position_interviewers
                                           where i.company_id == companyId && i.position_id == positionId
                                           select i).ToListAsync();

                foreach (var id in interviewersIds)
                {
                    if (dbInterviewer.Find(x => x.user_id == id) == null)
                    {
                        dbContext.position_interviewers.Add(new position_interviewer
                        {
                            company_id = companyId,
                            position_id = positionId,
                            user_id = id,
                        });
                    }
                }

                foreach (var item in dbInterviewer)
                {
                    if (Array.IndexOf(interviewersIds, item.user_id) == -1)
                    {
                        dbContext.position_interviewers.Remove(item);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task AddUpdatePositionContacts(int companyId, int positionId, List<int>? contactsIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                var conts = await (from i in dbContext.position_contacts
                                   where i.company_id == companyId && i.position_id == positionId
                                   select i).ToListAsync();

                foreach (var id in contactsIds)
                {
                    if (conts.Find(x => x.contact_id == id) == null)
                    {
                        dbContext.position_contacts.Add(new position_contact
                        {
                            company_id = companyId,
                            position_id = positionId,
                            contact_id = id,
                        });
                    }
                }

                foreach (var item in conts)
                {
                    int contId = contactsIds.Where(x => x == item.contact_id).FirstOrDefault();

                    if (contId == 0)
                    {
                        dbContext.position_contacts.Remove(item);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<ParserRulesModel>> GetParsersRules()
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from p in dbContext.company_parsers
                            join r in dbContext.parser_rules on p.parser_id equals r.parser_id
                            select new ParserRulesModel
                            {
                                company_id = p.company_id,
                                parser_id = r.parser_id,
                                delimiter = r.delimiter,
                                value_type = r.value_type,
                                must_metch = r.must_metch,
                                order = r.order,
                            };

                return await query.ToListAsync();
            }
        }

        public async Task<List<int>> GetCompaniesIds()
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.companies.Select(c => c.id).ToListAsync();
            }
        }

        public async Task<List<cv>> GetCompanyCvs(int companyId)
        {
            try
            {
                using (var dbContext = new cvup00001Context())
                {
                    List<cv> cvs = await dbContext.cvs.Where(x => x.company_id == companyId).ToListAsync();
                    return cvs;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<string?>> GetCompanyCvsIds(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.cvs.Where(x => x.company_id == companyId).Select(c => c.key_id).ToListAsync();
            }
        }

        public async Task<CvModel?> GetCv(int cvId, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from cand in dbContext.candidates
                            join cvs in dbContext.cvs on cand.id equals cvs.candidate_id
                            where cvs.id == cvId && cand.company_id == companyId
                            select new CvModel
                            {
                                candId = cand.id,
                                cvId = cvs.id,
                                //reviewHtml = cand.review_html,
                            };

                return await query.FirstOrDefaultAsync();
            }
        }

        public async Task SaveCandReview(int companyId, CandReviewModel candReview)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = dbContext.candidates.Where(x => x.company_id == companyId && x.id == candReview.candidateId).First();
                cand.review = candReview.review;
                cand.review_date = DateTime.Now;
                var result = dbContext.candidates.Update(cand);
                await dbContext.SaveChangesAsync();
            }
        }


        public async Task<cvs_txt?> CheckIsSameCv(int companyId, int candidateId, int cvAsciiSum)
        {
            using (var dbContext = new cvup00001Context())
            {
                cvs_txt? cvsTxt = await dbContext.cvs_txts.Where(x => x.company_id == companyId && x.candidate_id == candidateId && x.ascii_sum == cvAsciiSum).FirstOrDefaultAsync();
                return cvsTxt;
            }
        }

        public async Task AttachPosCandCv(AttachePosCandCvModel posCandCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                position_candidate? posCand = await dbContext.position_candidates.Where(x => x.company_id == posCandCv.companyId
                  && x.position_id == posCandCv.positionId
                  && x.candidate_id == posCandCv.candidateId).FirstOrDefaultAsync();

                string? posCvsStr = null;
                List<PosCvsModel>? posCvs = null;

                if (posCand != null && posCand.cand_cvs != null)
                {
                    posCvs = JsonConvert.DeserializeObject<List<PosCvsModel>>(posCand.cand_cvs);
                }

                if (posCvs == null)
                    posCvs = new List<PosCvsModel>();

                posCvs.Add(new PosCvsModel { cvId = posCandCv.cvId, isSentByEmail = false, keyId = posCandCv.keyId });
                posCvsStr = JsonConvert.SerializeObject(posCvs);

                var newPosCv = new position_candidate
                {
                    company_id = posCandCv.companyId,
                    position_id = posCandCv.positionId,
                    candidate_id = posCandCv.candidateId,
                    cv_id = posCandCv.cvId,
                    stage_id = 1,
                    cand_cvs = posCvsStr,
                    stage_date = DateTime.Now,
                    stage_type = "attached_to_position",
                    date_updated = DateTime.Now,
                };

                dbContext.position_candidates.Add(newPosCv);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DetachPosCand(AttachePosCandCvModel posCandCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                position_candidate? posCvs = dbContext.position_candidates.Where(x => x.company_id == posCandCv.companyId
                    && x.position_id == posCandCv.positionId
                    && x.candidate_id == posCandCv.candidateId
                    && x.cv_id == posCandCv.cvId).FirstOrDefault();

                if (posCvs != null)
                {
                    dbContext.position_candidates.Remove(posCvs);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateCandPosArrays(int companyId, int candidateId)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = dbContext.candidates.Where(x => x.company_id == companyId && x.id == candidateId).FirstOrDefault();

                if (cand != null)
                {
                    List<position_candidate>? candPosList = await dbContext.position_candidates.Where(x => x.company_id == companyId
                   && x.candidate_id == candidateId).ToListAsync();

                    List<CandPosStageModel> candPosStages = new List<CandPosStageModel>();
                    List<int> candPos = new List<int>();

                    foreach (var pcv in candPosList)
                    {
                        candPos.Add(pcv.position_id);
                        candPosStages.Add(new CandPosStageModel { _dt = pcv.stage_date?.ToString("yyyy-MM-dd"), _tp = pcv.stage_type, _pid = pcv.position_id, _ec = pcv.email_to_contact?.ToString("yyyy-MM-dd") });
                    }

                    var candPosStagesJson = JsonConvert.SerializeObject(candPosStages);

                    cand.pos_ids = $"[{string.Join(",", candPos)}]";
                    cand.pos_stages = candPosStagesJson;
                    var result = dbContext.candidates.Update(cand);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<company_cvs_email>> GetCompaniesEmails()
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.company_cvs_emails.ToListAsync();
            }
        }

        public async Task<List<CandPosStageTypeModel>> GetCandPosStagesTypes(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from st in dbContext.cand_pos_stages
                             where st.company_id == companyId
                             orderby st.order
                             select new CandPosStageTypeModel
                             {
                                 name = st.name,
                                 stageType = st.stage_Type,
                                 order = st.order,
                                 isCustom = Convert.ToBoolean(st.is_custom),
                                 color = st.color,
                                 stageEvent = st.stage_event
                             });

                return await query.ToListAsync();
            }
        }

        public async Task<List<EmailTemplateModel>> GetEmailTemplates(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from et in dbContext.emails_templates
                             orderby et.name
                             select new EmailTemplateModel
                             {
                                 id = et.id,
                                 name = et.name,
                                 subject = et.subject,
                                 body = et.body,
                                 stageToUpdate = et.stage_to_update
                             });

                return await query.ToListAsync();
            }
        }

        public async Task AddUpdateEmailTemplate(EmailTemplateModel emailTemplate)
        {
            using (var dbContext = new cvup00001Context())
            {
                if (emailTemplate.id == 0)
                {
                    dbContext.emails_templates.Add(new emails_template
                    {
                        name = emailTemplate.name,
                        subject = emailTemplate.subject,
                        stage_to_update = emailTemplate.stageToUpdate,
                        body = emailTemplate.body,
                    });
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    emails_template? etm = dbContext.emails_templates.Where(x => x.id == emailTemplate.id).First();
                    etm.name = emailTemplate.name;
                    etm.subject = emailTemplate.subject;
                    etm.stage_to_update = emailTemplate.stageToUpdate;
                    etm.body = emailTemplate.body;

                    var result = dbContext.emails_templates.Update(etm);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteEmailTemplate(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                emails_template? etm = dbContext.emails_templates.Where(x => x.id == id).FirstOrDefault();

                if (etm != null)
                {
                    dbContext.emails_templates.Remove(etm);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateCandDetails(CandDetailsModel candDetails)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = dbContext.candidates.Where(x => x.id == candDetails.candidateId).FirstOrDefault();

                if (cand != null)
                {
                    cand.first_name = candDetails.firstName;
                    cand.last_name = candDetails.lastName;
                    cand.email = candDetails.email;
                    cand.phone = candDetails.phone;
                    var result = dbContext.candidates.Update(cand);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateIsSeen(int companyId, int cvId)
        {
            using (var dbContext = new cvup00001Context())
            {
                cv? candCv = dbContext.cvs.Where(x => x.company_id == companyId && x.id == cvId).FirstOrDefault();

                if (candCv != null)
                {
                    candCv.is_seen = true;
                    var result = dbContext.cvs.Update(candCv);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<CandReportModel?>> CandsReport(int companyId, string stageType)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from pcs in dbContext.position_candidates
                             join cn in dbContext.candidates on pcs.candidate_id equals cn.id
                             join pos in dbContext.positions on pcs.position_id equals pos.id
                             where cn.company_id == companyId
                                    && pcs.stage_type == stageType
                             orderby pcs.stage_date descending
                             select new CandReportModel
                             {
                                 candidateId = cn.id,
                                 firstName = cn.first_name,
                                 lastName = cn.last_name,
                                 positionId = pos.id,
                                 positionName = pos.name,
                                 customerId = pos.customer_id,
                                 stageDate = pcs.stage_date,
                             }).Take(500); ;

                return await query.ToListAsync();
            }
        }

        public async Task<cand_pos_stage?> getPosStage(int companyId, string stageType)
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.cand_pos_stages.Where(x => x.company_id == companyId && x.stage_Type == stageType).FirstOrDefaultAsync();
            }
        }

        public async Task UpdateCandPositionStatus(CandPosStageTypeUpdateModel posStatus)
        {
            using (var dbContext = new cvup00001Context())
            {
                position_candidate? candPos = await dbContext.position_candidates.Where(x => x.company_id == posStatus.companyId
                  && x.candidate_id == posStatus.candidateId && x.position_id == posStatus.positionId).FirstOrDefaultAsync();

                if (candPos != null)
                {
                    //cand_pos_stage? posSatge = await getPosStage(posStatus.companyId, posStatus.stageType);

                    switch (posStatus.stageType)
                    {
                        case "sent_candidate_call_request":
                            candPos.call_email_to_candidate = DateTime.Now;
                            break;
                        case "cv_sent_to_customer":
                            candPos.email_to_contact = DateTime.Now;
                            break;
                        case "rejected_email_sent":
                            candPos.reject_email_to_candidate = DateTime.Now;
                            break;
                        case "customer_interview":
                            candPos.customer_interview = DateTime.Now;
                            break;
                        case "withdraw_candidacy":
                            candPos.remove_candidacy = DateTime.Now;
                            break;
                        case "rejected":
                            candPos.rejected = DateTime.Now;
                            break;
                        case "accepted":
                            candPos.accepted = DateTime.Now;
                            break;
                        default:
                            break;
                    }

                    candPos.stage_type = posStatus.stageType;
                    candPos.stage_date = DateTime.Now;
                    candPos.date_updated = DateTime.Now;

                    dbContext.position_candidates.Update(candPos);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdatePosStageDate(CandPosStageTypeUpdateModel posStatus)
        {
            using (var dbContext = new cvup00001Context())
            {
                position_candidate? candPos = await dbContext.position_candidates.Where(x => x.company_id == posStatus.companyId
                  && x.candidate_id == posStatus.candidateId && x.position_id == posStatus.positionId).FirstOrDefaultAsync();

                if (candPos != null)
                {
                    switch (posStatus.stageType)
                    {
                        case "sent_candidate_call_request":
                            candPos.call_email_to_candidate = posStatus.newDate;
                            break;
                        case "cv_sent_to_customer":
                            candPos.email_to_contact = posStatus.newDate;
                            break;
                        case "rejected_email_sent":
                            candPos.reject_email_to_candidate = posStatus.newDate;
                            break;
                        case "customer_interview":
                            candPos.customer_interview = posStatus.newDate;
                            break;
                        case "withdraw_candidacy":
                            candPos.remove_candidacy = posStatus.newDate;
                            break;
                        case "rejected":
                            candPos.rejected = posStatus.newDate;
                            break;
                        case "accepted":
                            candPos.accepted = posStatus.newDate;
                            break;
                        default:
                            break;
                    }

                    if (candPos.stage_type == posStatus.stageType)
                    {
                        candPos.stage_date = posStatus.newDate;
                    }

                    candPos.date_updated = DateTime.Now;

                    dbContext.position_candidates.Update(candPos);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task RemovePosStage(CandPosStageTypeUpdateModel posStatus)
        {
            using (var dbContext = new cvup00001Context())
            {
                position_candidate? candPos = await dbContext.position_candidates.Where(x => x.company_id == posStatus.companyId
                  && x.candidate_id == posStatus.candidateId && x.position_id == posStatus.positionId).FirstOrDefaultAsync();

                if (candPos != null)
                {
                    switch (posStatus.stageType)
                    {
                        case "sent_candidate_call_request":
                            candPos.call_email_to_candidate = null;
                            break;
                        case "cv_sent_to_customer":
                            candPos.email_to_contact = null;
                            break;
                        case "rejected_email_sent":
                            candPos.reject_email_to_candidate = null;
                            break;
                        case "customer_interview":
                            candPos.customer_interview = null;
                            break;
                        case "withdraw_candidacy":
                            candPos.remove_candidacy = null;
                            break;
                        case "rejected":
                            candPos.rejected = null;
                            break;
                        case "accepted":
                            candPos.accepted = null;
                            break;
                        default:
                            break;
                    }

                    if (candPos.stage_type == posStatus.stageType)
                    {
                        candPos.stage_type = "attached_to_position";
                        candPos.stage_date = DateTime.Now;
                    }

                    candPos.date_updated = DateTime.Now;
                    dbContext.position_candidates.Update(candPos);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        //public async Task updateCandPosCallEmailToCandidate(int companyId, int candidateId, int positionId)
        //{
        //    using (var dbContext = new cvup00001Context())
        //    {
        //        position_candidate? candPos = await dbContext.position_candidates.Where(x => x.company_id == companyId && x.candidate_id == candidateId && x.position_id == positionId).FirstOrDefaultAsync();

        //        if (candPos != null)
        //        {
        //            candPos.call_email_to_candidate = DateTime.Now;
        //            dbContext.position_candidates.Update(candPos);
        //            await dbContext.SaveChangesAsync();
        //        }
        //    }
        //}

        //public async Task updateCandPosEmailToCustomer(int companyId, int candidateId, int positionId)
        //{
        //    using (var dbContext = new cvup00001Context())
        //    {
        //        position_candidate? candPos = await dbContext.position_candidates.Where(x => x.company_id == companyId && x.candidate_id == candidateId && x.position_id == positionId).FirstOrDefaultAsync();

        //        if (candPos != null)
        //        {
        //            candPos.email_to_contact = DateTime.Now;
        //            dbContext.position_candidates.Update(candPos);
        //            await dbContext.SaveChangesAsync();
        //        }
        //    }
        //}
        //public async Task updateCandPosRejectEmailToCandidate(int companyId, int candidateId, int positionId)
        //{
        //    using (var dbContext = new cvup00001Context())
        //    {
        //        position_candidate? candPos = await dbContext.position_candidates.Where(x => x.company_id == companyId && x.candidate_id == candidateId && x.position_id == positionId).FirstOrDefaultAsync();

        //        if (candPos != null)
        //        {
        //            candPos.reject_email_to_candidate = DateTime.Now;
        //            dbContext.position_candidates.Update(candPos);
        //            await dbContext.SaveChangesAsync();
        //        }
        //    }
        //}

        public async Task UpdatePositionDate(int companyId, int positionId, bool isUpdateCount)
        {
            using (var dbContext = new cvup00001Context())
            {
               

                position? pos = dbContext.positions.Where(x => x.company_id == companyId && x.id == positionId).First();
                pos.date_updated = DateTime.Now;
                if (isUpdateCount)
                {
                    var count = dbContext.position_candidates.Where(x => x.company_id == companyId && x.position_id == positionId).Count();
                    pos.cands_count = count;
                }
                var result = dbContext.positions.Update(pos);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<position?> GetPositionByMatchStr(int companyId, string matchStr)
        {
            using (var dbContext = new cvup00001Context())
            {
                List<position> posList = await dbContext.positions.Where(x => x.company_id == companyId && !string.IsNullOrWhiteSpace(x.match_email_subject) && x.status == "Active").ToListAsync();

                position? matchPos = posList.Where(x => !string.IsNullOrWhiteSpace(x.match_email_subject) && matchStr.Contains(x.match_email_subject)).FirstOrDefault();

                return matchPos;
            }
        }

        public async Task AddSendEmail(SendEmailModel emailData, int userId)
        {
            string toAddresses = "";

            if (emailData.toAddresses != null)
            {
                foreach (var item in emailData.toAddresses)
                {
                    toAddresses += item.Address + ", ";
                }
            }

            toAddresses = toAddresses.Substring(0, toAddresses.Length - 2);

            using (var dbContext = new cvup00001Context())
            {
                dbContext.sent_emails.Add(new sent_email
                {
                    company_id = emailData.companyId,
                    candidate_id = emailData.candidateId,
                    position_id = emailData.positionId,
                    cv_id = emailData.cvId,
                    subject = emailData.subject != null ? emailData.subject.Substring(0, Math.Min(emailData.subject.Length, 500)) : null,
                    body = emailData.body != null ? emailData.body.Substring(0, Math.Min(emailData.body.Length, 1000)) : null,
                    user_id = userId,
                    to = toAddresses.Substring(0, Math.Min(toAddresses.Length, 500))
                });

                await dbContext.SaveChangesAsync();

            }
        }

        public async Task<int?> GetPositionTypeId(int companyId, string positionRelated)
        {
            using (var dbContext = new cvup00001Context())
            {
                var posType = await dbContext.position_types.Where(x => x.company_id == companyId && x.type_name == positionRelated).FirstOrDefaultAsync();

                if (posType != null)
                {
                    posType.date_updated = DateTime.Now;
                    dbContext.position_types.Update(posType);
                    await dbContext.SaveChangesAsync();

                    return posType.id;
                }

                return null;
            }
        }

        public async Task<int> AddPositionTypeName(int companyId, string positionRelated)
        {
            using (var dbContext = new cvup00001Context())
            {

                var result = dbContext.position_types.Add(new position_type
                {
                    company_id = companyId,
                    type_name = positionRelated,
                    date_updated = DateTime.Now
                });

                await dbContext.SaveChangesAsync();

                return result.Entity.id;
            }
        }

        public async Task<List<PositionTypeModel>> GetPositionsTypes(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from et in dbContext.position_types
                             where et.company_id == companyId
                             orderby et.date_updated descending
                             select new PositionTypeModel
                             {
                                 id = et.id,
                                 typeName = et.type_name,
                                 dateUpdated = et.date_updated,
                             });

                return await query.ToListAsync();


            }
        }

        public async Task SaveCustomerCandReview(int companyId, CandReviewModel customerCandReview)
        {
            using (var dbContext = new cvup00001Context())
            {
                position_candidate? posCand = await dbContext.position_candidates.Where(x => x.company_id == companyId
                && x.candidate_id == customerCandReview.candidateId
                && x.position_id == customerCandReview.positionId).FirstOrDefaultAsync();

                if (posCand != null)
                {
                    posCand.customer_review = customerCandReview.review.Trim();
                    posCand.date_updated = DateTime.Now;

                    var result = dbContext.position_candidates.Update(posCand);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateCandCustomersReviews(int companyId, int candidateId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from p in dbContext.positions
                            join c in dbContext.customers on p.customer_id equals c.id
                            join pc in dbContext.position_candidates on p.id equals pc.position_id
                            where p.company_id == companyId && pc.candidate_id == candidateId && !string.IsNullOrEmpty(pc.customer_review)
                            orderby pc.date_updated descending
                            select new CandCustomersReviewsModel
                            {
                                candId = pc.candidate_id,
                                posId = pc.position_id,
                                custId = c.id,
                                posName = p.name,
                                custName = c.name,
                                review = pc.customer_review,
                                updated = pc.date_updated,
                            };

                var CandCustomersReviewsList = await query.ToListAsync();

                if (CandCustomersReviewsList.Count > 0)
                {
                    var candCustomersReviews = JsonConvert.SerializeObject(CandCustomersReviewsList);

                    candidate? cand = dbContext.candidates.Where(x => x.id == candidateId).FirstOrDefault();

                    if (cand != null)
                    {
                        cand.customers_reviews = candCustomersReviews;

                        var result = dbContext.candidates.Update(cand);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task CalculatePositionTypesCount(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                List<cv> cvsList = await dbContext.cvs.Where(x => x.company_id == companyId && x.date_created >= DateTime.Now.AddDays(-1)).ToListAsync();

                var posTypesIds = cvsList.Select(x => x.position_type_id).Distinct().ToList();
                posTypesIds.Remove(posTypesIds.Where(x => x == null).FirstOrDefault());

                var query = (from p in dbContext.position_types
                             where p.company_id == companyId && posTypesIds.Contains(p.id)
                             select p);

                var positionTypesList = await query.ToListAsync();

                foreach (var pt in posTypesIds)
                {
                    int count1 = 0, count2 = 0;

                    foreach (var cv in cvsList)
                    {
                        if (cv.position_type_id == pt)
                        {
                            if (cv.date_created.Date == DateTime.Now.AddDays(-1).Date)
                            {
                                count2++;
                            }
                            else
                            {
                                count1++;
                            }
                        }
                    }

                    var posType = positionTypesList.Where(x => x.id == pt).First();

                    posType.cvs_today = count1;
                    posType.cvs_yesterday = count2;

                }

                dbContext.position_types.UpdateRange(positionTypesList);
                await dbContext.SaveChangesAsync();

            }
        }

        public async Task<List<PositionTypeCountModel>> PositionsTypesCvsCount(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from p in dbContext.position_types
                             where p.company_id == companyId && p.date_updated >= DateTime.Now.AddDays(-1)
                             orderby p.date_updated descending
                             select new PositionTypeCountModel
                             {
                                 id = p.id,
                                 typeName = p.type_name,
                                 todayCount = p.cvs_today,
                                 yesterdayCount = p.cvs_yesterday
                             });

                var ptList = await query.ToListAsync();

                return ptList;
            }
        }
    }
}
