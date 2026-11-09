-- The kind of an email template. The allowed values match the
-- EmailTemplateType enum in DataModelsLibrary/Enums/Enums.cs.
-- All the existing templates are candidate emails.

BEGIN;

ALTER TABLE public.emails_templates
    ADD COLUMN IF NOT EXISTS template_type varchar(50);

UPDATE public.emails_templates
SET template_type = 'cand_email'
WHERE template_type IS NULL;

ALTER TABLE public.emails_templates
    ALTER COLUMN template_type SET DEFAULT 'cand_email',
    ALTER COLUMN template_type SET NOT NULL;

ALTER TABLE public.emails_templates
    ADD CONSTRAINT ck_emails_templates_template_type
    CHECK (template_type IN ('cand_email', 'contract_email'));

COMMIT;
