-- The appointments table was renamed to schedule_interviews.
-- Run once on an installation that was created by the old create_appointments.sql.

ALTER TABLE IF EXISTS public.appointments
    RENAME TO schedule_interviews;

ALTER INDEX IF EXISTS public.appointments_pkey
    RENAME TO schedule_interviews_pkey;

ALTER INDEX IF EXISTS public.ix_appointments_company_id_start_time
    RENAME TO ix_schedule_interviews_company_id_start_time;

ALTER INDEX IF EXISTS public.ix_appointments_position_id
    RENAME TO ix_schedule_interviews_position_id;

ALTER INDEX IF EXISTS public.ix_appointments_candidate_id
    RENAME TO ix_schedule_interviews_candidate_id;

ALTER SEQUENCE IF EXISTS public.appointments_id_seq
    RENAME TO schedule_interviews_id_seq;
