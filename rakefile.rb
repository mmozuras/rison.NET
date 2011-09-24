# encoding: utf-8
require 'rubygems'
require 'albacore'
require 'rake/clean'
require 'rexml/document'

OUTPUT = "build"
CONFIGURATION = 'Release'
SOLUTION_FILE = 'rison.NET.sln'

Albacore.configure do |config|
    config.log_level = :verbose
    config.msbuild.use :net4
end

desc "Compiles solution and runs unit tests"
task :default => [:clean, :compile, :test, :publish]

#Add the folders that should be cleaned as part of the clean task
CLEAN.include(OUTPUT)
CLEAN.include(FileList["src/**/#{CONFIGURATION}"])

desc "Compile solution file"
msbuild :compile => [:clean] do |msb|
    msb.properties :configuration => CONFIGURATION
    msb.targets :Clean, :Build
    msb.solution = SOLUTION_FILE
end

desc "Gathers output files and copies them to the output folder"
task :publish => [:compile] do
    Dir.mkdir(OUTPUT)
    Dir.mkdir("#{OUTPUT}/binaries")

    FileUtils.cp_r FileList["src/**/#{CONFIGURATION}/*.dll", "src/**/#{CONFIGURATION}/*.pdb"].exclude(/obj\//).exclude(/.Tests/), "#{OUTPUT}/binaries"
end

desc "Executes NUnit tests"
nunit :test => [:compile] do |nunit|
    tests = FileList["src/**/#{CONFIGURATION}/*.Tests.dll"].exclude(/obj\//)

    nunit.command = "packages/NUnit.2.5.10.11092/tools/nunit-console.exe"
    nunit.assemblies = tests
end
