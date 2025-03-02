ALTER TABLE serilog
ADD COLUMN source_context TEXT 
GENERATED ALWAYS AS (log_event -> 'Properties' ->> 'SourceContext') STORED;


CREATE INDEX idx_serilog_source_context ON serilog (source_context);