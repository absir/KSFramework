#!/usr/bin/env bash
project=$1
if [ -z $project ];then
	printf "需要工程名称"
	exit
fi

templete="Templetes/Templete"

command="cp -r $templete Projects/$project"
printf "$command\r\n"
$command

command="rm -rf Projects/$project/Assets/System"
printf "$command\r\n"
$command

command="ln -s ../../../$templete/Assets/System Projects/$project/Assets/System"
printf "$command\r\n"
$command