-- A log of the contracts sent to customers.
-- The values are copied at send time, so a row stays accurate after the
-- template, customer or contact is changed or deleted.

CREATE TABLE public.contract_sends
(
    id                      integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    contract_template_name  text      NOT NULL,
    contract_template_descr text      NULL,
    company_name            text      NOT NULL,
    contact_name            text      NULL,
    email_address           text      NOT NULL,
    email_subject           text      NULL,
    email_body              text      NULL,
    date_sent               timestamp without time zone NOT NULL DEFAULT now()
);

CREATE INDEX ix_contract_sends_date_sent
    ON public.contract_sends (date_sent);
