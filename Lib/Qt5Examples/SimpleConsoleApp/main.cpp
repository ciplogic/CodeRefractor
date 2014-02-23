#include <QCoreApplication>

#include <QString>
#include <QString>
#include <QDebug>

#include <iostream>

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);

    QString text = "qt string is text";
    auto wText = text.toStdWString();

    std::cout << text.toStdString() << std::endl;

    return a.exec();
}
