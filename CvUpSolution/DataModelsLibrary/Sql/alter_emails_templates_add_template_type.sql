-- Run after create_customer_contracts.sql, then re-scaffold the EF models.
--
-- Tells the send form which email templates are contract emails:
-- 'candidate' | 'contact' | 'contract'.
ALTER TABLE public.emails_templates
    ADD COLUMN template_type character varying(50) NULL;
