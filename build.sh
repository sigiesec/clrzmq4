#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

msbuild /p:Configuration=Release clrzmq4.mono.sln

export MONO_TRACE_LISTENER=Console.Out

# MONO_OPTIONS="--profile=monocov:outfile=ZeroMQ.cov,+[ZeroMQ]"
export MONO_LOG_LEVEL=debug
export MONO_LOG_MASK=dll,cfg
if [ "$(uname)" == "Darwin" ] ; then
  cp amd64/libzmq.dylib ZeroMQTest/bin/Release
fi
mono ./testrunner/NUnit.ConsoleRunner.3.6.1/tools/nunit3-console.exe ./ZeroMQTest/bin/Release/ZeroMQTest.dll

# monocov --export-xml=ZeroMQ.cov.xml ZeroMQ.cov
# REPO_COMMIT_AUTHOR=$(git show -s --pretty=format:"%cn")
# REPO_COMMIT_AUTHOR_EMAIL=$(git show -s --pretty=format:"%ce")
# REPO_COMMIT_MESSAGE=$(git show -s --pretty=format:"%s")
# mono .\\tools\\coveralls.net.0.7.0\\tools\\csmacnz.Coveralls.exe --monocov -i ./ZeroMQ.cov.xml --repoToken $COVERALLS_REPO_TOKEN --commitId $TRAVIS_COMMIT --commitBranch $TRAVIS_BRANCH --commitAuthor "$REPO_COMMIT_AUTHOR" --commitEmail "$REPO_COMMIT_AUTHOR_EMAIL" --commitMessage "$REPO_COMMIT_MESSAGE" --jobId $TRAVIS_JOB_ID  --serviceName travis-ci  --useRelativePaths

revision=${TRAVIS_JOB_ID:=1}
revision=$(printf "%04d" $revision)

#nuget pack ./src/Invio.Extensions.DependencyInjection -c Release -o ./artifacts
