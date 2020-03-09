# -*- coding: utf-8 -*-
#
# MongoDB documentation build configuration file, created by
# sphinx-quickstart on Mon Oct  3 09:58:40 2011.
#
# This file is execfile()d with the current directory set to its containing dir.

import sys
import os.path
import datetime

project_root = os.path.join(os.path.abspath(os.path.dirname(__file__)))
sys.path.append(project_root)

from giza.config.runtime import RuntimeStateConfig
from giza.config.helper import fetch_config, get_versions, get_manual_path

conf = fetch_config(RuntimeStateConfig())
intersphinx_libs = conf.system.files.data.intersphinx
sconf = conf.system.files.data.sphinx_local

sys.path.append(os.path.join(conf.paths.projectroot, conf.paths.buildsystem, 'sphinxext'))

# -- General configuration ----------------------------------------------------

needs_sphinx = '1.0'

extensions = [
    'sphinx.ext.extlinks',
    'sphinx.ext.todo',
    'mongodb',
    'directives',
    'intermanual',
    # 'fasthtml',
    'tabs',
    'tab-content',
    'stitch-builders',
]

templates_path = ['.templates']
exclude_patterns = []

source_suffix = '.txt'

master_doc = sconf.master_doc
language = 'en'
project = sconf.project
copyright = u'2008-{0}'.format(datetime.date.today().year)

version = '3.0'
release = version

rst_epilog = '\n'.join([
    '.. |branch| replace:: ``{0}``'.format(conf.git.branches.current),
    '.. |copy| unicode:: U+000A9',
    '.. |year| replace:: {0}'.format(datetime.date.today().year),
    '.. |ent-build| replace:: MongoDB Enterprise',
    '.. |hardlink| replace:: http://docs.mongodb.com/realm/',
    '.. |atlas-full| replace:: MongoDB Atlas',
    '.. |atlas| replace:: Atlas'
])

extlinks = {
    # MongoDB Docs Sites
    'manual': ('http://docs.mongodb.org/manual%s', ''),
    'atlas': ('https://docs.atlas.mongodb.com%s',''),
    'fb-dev-docs': ('https://developers.facebook.com/docs/%s', ''),
    'mms-docs': ('https://docs.cloud.mongodb.com%s', ''),
    'mms-home': ('https://cloud.mongodb.com%s', ''),
    'guides': ('https://docs.mongodb.com/guides%s', ''),
    # True External Links
    'android-sdk': ('https://docs.mongodb.com/stitch-sdks/java/4/%s', ''),
    'apollo-docs': ('https://www.apollographql.com/docs%s', ''),
    'codesandbox': ('https://codesandbox.io/%s', ''),
    'gcp': ('https://cloud.google.com/%s', ''),
    'github': ('https://github.com/%s', ''),
    'google-dev': ('https://developers.google.com/%s', ''),
    'graphql': ('https://graphql.org/%s', ''),
    'ios-sdk': ('https://docs.mongodb.com/stitch-sdks/swift/6/%s', ''),
    'js-sdk': ('https://docs.mongodb.com/stitch-sdks/js/4/%s', ''),
    'jwt-io': ('https://jwt.io/%s', ''),
    'nodejs': ('https://nodejs.org/api/%s', ''),
    'npm': ('https://www.npmjs.com/%s', ''),
    'mdn': ('https://developer.mozilla.org/en-US/docs/%s', ''),
    'wikipedia': ('https://en.wikipedia.org/wiki/%s', ''),
    'aws-docs': ('https://docs.aws.amazon.com/%s', ''),
}

intersphinx_mapping = {}

try:
    for i in intersphinx_libs:
        intersphinx_mapping[i['name']] = (i['url'], os.path.join(conf.paths.projectroot,
                                                                 conf.paths.output,
                                                                 i['path']))
except:
    for i in intersphinx_libs:
        intersphinx_mapping[i.name] = (i.url, os.path.join(conf.paths.projectroot,
                                                           conf.paths.output,
                                                           i.path))


languages = [
    ("ar", "Arabic"),
    ("cn", "Chinese"),
    ("cs", "Czech"),
    ("de", "German"),
    ("es", "Spanish"),
    ("fr", "French"),
    ("hu", "Hungarian"),
    ("id", "Indonesian"),
    ("it", "Italian"),
    ("jp", "Japanese"),
    ("ko", "Korean"),
    ("lt", "Lithuanian"),
    ("pl", "Polish"),
    ("pt", "Portuguese"),
    ("ro", "Romanian"),
    ("ru", "Russian"),
    ("tr", "Turkish"),
    ("uk", "Ukrainian")
]

# -- Options for HTML output ---------------------------------------------------

html_theme = sconf.theme.name
html_theme_path = [ os.path.join(conf.paths.buildsystem, 'themes') ]
html_title = conf.project.title
htmlhelp_basename = 'MongoDBdoc'

html_logo = ".static/logo-mongodb.png"
html_static_path = ['source/.static']
html_last_updated_fmt = '%b %d, %Y'

html_copy_source = False
html_domain_indices = True
html_use_index = True
html_split_index = False
html_show_sourcelink = False
html_show_sphinx = True
html_show_copyright = True

manual_edition_path = '{0}/{1}/{2}.{3}'

html_theme_options = {
    'branch': conf.git.branches.current,
    'translations': languages,
    'language': language,
    'manual_path': "ecosystem",
    'repo_name': 'docs-ecosystem',
    'jira_project': 'DOCS',
    'google_analytics': sconf.theme.google_analytics,
    'project': sconf.project,
    'nav_excluded': sconf.theme.nav_excluded,
}

html_sidebars = sconf.sidebars
