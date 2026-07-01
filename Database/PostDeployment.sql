/*
Script posterior a la implementación.

Este archivo se ejecuta después de publicar el esquema de la base.
Desde acá se incluyen los scripts de datos iniciales obligatorios.
*/

:r .\Scripts\Seed\001_idiomas.sql
:r .\Scripts\Seed\002_seguridad_basica.sql
:r .\Scripts\Seed\003_integridad_inicial.sql
:r .\Scripts\Seed\004_traducciones_es_ar.sql
:r .\Scripts\Seed\005_traducciones_en_us.sql
:r .\Scripts\Seed\006_traducciones_pt_br.sql