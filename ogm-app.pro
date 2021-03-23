QT       += core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

CONFIG += c++11 

# You can make your code fail to compile if it uses deprecated APIs.
# In order to do so, uncomment the following line.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

SOURCES += \
    code/app.cpp \
    code/main.cpp


HEADERS += \
    code/app.h

FORMS += \
    ui/app.ui

#指定生成的应用程序名
TARGET = OGM

#程序编译时依赖的相关路径
DEPENDPATH += ./3rd
#头文件包含路径
INCLUDEPATH += ./3rd


