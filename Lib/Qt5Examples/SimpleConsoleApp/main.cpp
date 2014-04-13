#include <QCoreApplication>

#include <QString>
#include <QDebug>
#include <QFile>

#include <iostream>

void ReadAllLines(){
    QFile inputFile(":/input.txt");
    if (inputFile.open(QIODevice::ReadOnly))
    {
       QTextStream in(&inputFile);
       QVector<QString> lines;
       while ( !in.atEnd() )
       {
          auto line = in.readLine();
          lines.append(line);
          in.readAll()

       }
       inputFile.close();
    }
}

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);

    QString text = "qt string is text";
    auto wText = text.toStdWString();

    std::cout << text.toStdString() << std::endl;

    return a.exec();
}
