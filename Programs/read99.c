#include <stdio.h>
#include <string.h>

int main(int argc,char** argv) {
  FILE* inFile;
  char  buffer[256];
  char  num[20];
  int   n;
  inFile = fopen("eniac.out","r");
  if (inFile != NULL) {
    while (fgets(buffer,255,inFile) != NULL) {
      strncpy(num,buffer+1,10);
      num[10] = 0;
      n = atoi(num);
      if (n != 10) printf("%c",n); else printf("\n");
      }
    }
  }

