SQL Queries
---------------


SELECT 	cand.id,`email`, `name`, COUNT(*) cn
FROM `cvup00001`.`candidates` cand INNER JOIN cvs ON cand.id = cvs.candidate_id
GROUP BY `id`, `email`,`name`, `first_name`, `last_name`
HAVING COUNT(*) BETWEEN  30 AND  50
ORDER BY COUNT(*)
	

INSERT INTO  black_cands (candidate_id,email,phone,`name`,cvs_count)
 SELECT  candidates.id candidate_id, 
	candidates.email , 
	candidates.phone , 
	candidates.name , 
	COUNT(*) cvs_count
	FROM 
	candidates INNER JOIN cvs ON candidates.id = cvs.candidate_id
	GROUP BY candidate_id, 
	email, 
	`name`
	HAVING cvs_count >=  30 



UPDATE candidates AS dest ,
(SELECT candidate_id FROM black_cands) AS src
SET dest.is_black_list = 1
WHERE dest.id = src.candidate_id


