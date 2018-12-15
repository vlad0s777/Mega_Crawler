SELECT run_initial_if_migration_table_is_empty('
		CREATE TABLE IF NOT EXISTS public."ArticleTag" (
			"TagId" integer NOT NULL,
			"ArticleId" integer NOT NULL
		);


		ALTER TABLE public."ArticleTag" OWNER TO postgres;

		CREATE TABLE IF NOT EXISTS public."Articles" (
			"ArticleId" integer NOT NULL,
			"DateCreate" timestamp without time zone NOT NULL,
			"Text" text,
			"Head" text,
			"OuterArticleId" integer DEFAULT 0 NOT NULL
		);


		ALTER TABLE public."Articles" OWNER TO postgres;

		CREATE SEQUENCE IF NOT EXISTS public."Articles_ArticleId_seq"
			AS integer
			START WITH 1
			INCREMENT BY 1
			NO MINVALUE
			NO MAXVALUE
			CACHE 1;


		ALTER TABLE public."Articles_ArticleId_seq" OWNER TO postgres;

		ALTER SEQUENCE public."Articles_ArticleId_seq" OWNED BY public."Articles"."ArticleId";

		CREATE TABLE IF NOT EXISTS public."RemovedTags" (
			"RemovedTagId" integer NOT NULL,
			"TagId" integer NOT NULL,
			"DeletionDate" timestamp without time zone NOT NULL
		);


		ALTER TABLE public."RemovedTags" OWNER TO postgres;

		CREATE TABLE IF NOT EXISTS public."Tags" (
			"TagId" integer NOT NULL,
			"TagKey" text,
			"Name" text
		);


		ALTER TABLE public."Tags" OWNER TO postgres;

		CREATE SEQUENCE IF NOT EXISTS public."TagsDelete_TagDeleteId_seq"
			AS integer
			START WITH 1
			INCREMENT BY 1
			NO MINVALUE
			NO MAXVALUE
			CACHE 1;


		ALTER TABLE public."TagsDelete_TagDeleteId_seq" OWNER TO postgres;

		ALTER SEQUENCE public."TagsDelete_TagDeleteId_seq" OWNED BY public."RemovedTags"."RemovedTagId";

		CREATE SEQUENCE IF NOT EXISTS public."Tags_TagId_seq"
			AS integer
			START WITH 1
			INCREMENT BY 1
			NO MINVALUE
			NO MAXVALUE
			CACHE 1;

		ALTER TABLE public."Tags_TagId_seq" OWNER TO postgres;

		ALTER SEQUENCE public."Tags_TagId_seq" OWNED BY public."Tags"."TagId";

		ALTER TABLE ONLY public."Articles" ALTER COLUMN "ArticleId" SET DEFAULT nextval(''public."Articles_ArticleId_seq"''::regclass);

		ALTER TABLE ONLY public."RemovedTags" ALTER COLUMN "RemovedTagId" SET DEFAULT nextval(''public."TagsDelete_TagDeleteId_seq"''::regclass);

		ALTER TABLE ONLY public."Tags" ALTER COLUMN "TagId" SET DEFAULT nextval(''public."Tags_TagId_seq"''::regclass);

		SELECT create_constraint_if_not_exists(
				''ArticleTag'',
				''PK_ArticleTag'',
				''ALTER TABLE ONLY public."ArticleTag" ADD CONSTRAINT "PK_ArticleTag" PRIMARY KEY ("ArticleId", "TagId")'');
				
		SELECT create_constraint_if_not_exists(
				''Articles'',
				''PK_Articles'',
				''ALTER TABLE ONLY public."Articles" ADD CONSTRAINT "PK_Articles" PRIMARY KEY ("ArticleId")'');

		SELECT create_constraint_if_not_exists(
				''RemovedTags'',
				''PK_RemovedTags'',
				''ALTER TABLE ONLY public."RemovedTags" ADD CONSTRAINT "PK_RemovedTags" PRIMARY KEY ("RemovedTagId")'');
				
		SELECT create_constraint_if_not_exists(
				''Tags'',
				''PK_Tags'',
				''ALTER TABLE ONLY public."Tags" ADD CONSTRAINT "PK_Tags" PRIMARY KEY ("TagId")'');
				

		CREATE INDEX IF NOT EXISTS "IX_ArticleTag_TagId" ON public."ArticleTag" USING btree ("TagId");

		CREATE UNIQUE INDEX IF NOT EXISTS "IX_Articles_OuterArticleId" ON public."Articles" USING btree ("OuterArticleId");

		CREATE UNIQUE INDEX IF NOT EXISTS "IX_RemovedTags_TagId" ON public."RemovedTags" USING btree ("TagId");


		SELECT create_constraint_if_not_exists(
				''Articles'',
				''FK_ArticleTag_Articles_ArticleId'',
				''ALTER TABLE ONLY public."ArticleTag" ADD CONSTRAINT "FK_ArticleTag_Articles_ArticleId" FOREIGN KEY ("ArticleId") REFERENCES public."Articles"("ArticleId") ON DELETE CASCADE'');

		SELECT create_constraint_if_not_exists(
				''Tags'',
				''FK_ArticleTag_Tags_TagId'', 
				''ALTER TABLE ONLY public."ArticleTag" ADD CONSTRAINT "FK_ArticleTag_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES public."Tags"("TagId") ON DELETE CASCADE'');

		SELECT create_constraint_if_not_exists(
				''Tags'',
				''FK_RemovedTags_Tags_TagId'', 
				''ALTER TABLE ONLY public."RemovedTags"
			ADD CONSTRAINT "FK_RemovedTags_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES public."Tags"("TagId") ON DELETE RESTRICT'');');