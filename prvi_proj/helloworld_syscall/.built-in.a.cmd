cmd_hello/built-in.a := rm -f hello/built-in.a;  printf "hello/%s " hello.o | xargs ar cDPrST hello/built-in.a
