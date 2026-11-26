#include <QApplication>

#include <exception>
#include <iostream>

extern "C" void doroti_qt_test_semantics();

int main(int argc, char** argv) {
  QApplication app(argc, argv);
  try {
    doroti_qt_test_semantics();
    std::cout << "PASS: native Qt caption identity, stable topology, bounds and text rectangles\n";
    return 0;
  } catch (const std::exception& error) {
    std::cerr << error.what() << '\n';
    return 1;
  }
}
