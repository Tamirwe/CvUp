-- The customer's address and company id (business registration number) as
-- entered when the contract was sent. Both are optional, so older rows stay null.
ALTER TABLE contract_sends
    ADD COLUMN IF NOT EXISTS company_address text NULL,
    ADD COLUMN IF NOT EXISTS company_id_number text NULL;
