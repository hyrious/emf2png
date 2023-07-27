
all: emf2png.exe

csc.cmd: make-csc.cmd
	$^

emf2png.exe: emf2png.cs csc.cmd
	csc.cmd /nologo /o emf2png.cs

clean:
	rm -f csc.cmd emf2png.exe
