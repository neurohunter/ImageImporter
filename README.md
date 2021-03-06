# ImageImporter

[![Build Status](https://vishnyakovpavel.visualstudio.com/ImageImporter/_apis/build/status/Create-Release?branchName=master)](https://vishnyakovpavel.visualstudio.com/ImageImporter/_build/latest?definitionId=7&branchName=master)

## Introduction

ImageImporter - a small command-line utility to quickly import images from one directory (e.g. camera memory card) to other directory while sorting images in the process.
Can be used either by providing all the necessary arguments via command line or by providing an input directory and a config file that describes how to process the images.

## Usage

### Command-line arguments

        c:configuration-path            Path to a configuration xml file
        i:input-directory               Input directory with image files
        n:non-raw-types                 Extensions for non-RAW images (e.g. JPEG) (i.e. -r .ext1 .ext2)
        o:output-directory              Directory to put files to (will be created if absent)
        r:raw-types                     Extensions for RAW images (e.g. -r .ext1 .ext2)
        v:video-types                   Extensions for video files (i.e. -r .ext1 .ext2)

### Configuration file

*   if c | configuration-path parameter is provided, an attempt will be made to get configuration from that file. If other arguments are provided, they will override values of the configuration file

The following can be configured in a configuration file:

* destination directory
* file extensions to be treated as RAW (files with these extensions will be put into `<destination directory>\RAW`)
* file extensions to be treated as non-RAW (files with these extensions will be put into `<destination directory>\JPG`)
* file extensions to be treated as video (files with these extensions will be put into `<destination directory>\VIDEO`)



## Features

### 1.0.x

* Imports images from a specified directory into a destination directory
* sorts images into different directories based on date taken from EXIF
* sorts other files from the same directory based on last access date
* videos currently not supported / tested, planned for future

### 1.x

TBD


## Getting Started

TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:

1. Installation process
2. Software dependencies
3. Latest releases
4. API references

## Build and Test

TODO: Describe and show how to build your code and run the tests.
