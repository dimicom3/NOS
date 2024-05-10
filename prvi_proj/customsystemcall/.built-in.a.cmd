cmd_customsystemcall/built-in.a := rm -f customsystemcall/built-in.a;  printf "customsystemcall/%s " customsystemcall.o | xargs ar cDPrST customsystemcall/built-in.a
