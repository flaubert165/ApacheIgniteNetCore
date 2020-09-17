cd /u01/app/oracle/admin/XE/dpdump/
impdp system/oracle dumpfile=dumpclear.dmp logfile=impdatabase.log full=y
impdp clear/clear dumpfile=sinacormetadata.dmp logfile=impdatabase.log full=y
