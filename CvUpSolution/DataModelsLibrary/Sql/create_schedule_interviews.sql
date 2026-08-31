-- Interviews scheduled from the cv view.
-- The guests are kept as jsonb arrays of { "Name": "", "Address": "" }.

CREATE TABLE IF NOT EXISTS public.schedule_interviews
(
    id                    integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    company_id            integer     NOT NULL,
    user_id               integer     NOT NULL,
    position_id           integer     NULL,
    candidate_id          integer     NULL,
    subject               text        NOT NULL DEFAULT '',
    start_time            timestamptz NOT NULL,
    duration_minutes      integer     NOT NULL DEFAULT 60,
    location              text        NULL,
    notes                 text        NULL,
    candidates            jsonb       NOT NULL DEFAULT '[]'::jsonb,
    contacts              jsonb       NOT NULL DEFAULT '[]'::jsonb,
    optional_participants jsonb       NOT NULL DEFAULT '[]'::jsonb,
    ics_uid               text        NOT NULL,
    ics_sequence          integer     NOT NULL DEFAULT 0,
    created_at            timestamptz NOT NULL DEFAULT now(),
    updated_at            timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_schedule_interviews_company_id_start_time
    ON public.schedule_interviews (company_id, start_time);

CREATE INDEX IF NOT EXISTS ix_schedule_interviews_position_id
    ON public.schedule_interviews (position_id);

CREATE INDEX IF NOT EXISTS ix_schedule_interviews_candidate_id
    ON public.schedule_interviews (candidate_id);

-- Existing installations.
ALTER TABLE public.schedule_interviews
    ADD COLUMN IF NOT EXISTS ics_sequence integer NOT NULL DEFAULT 0;

