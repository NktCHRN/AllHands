import type { Metadata, ResolvingMetadata } from 'next'
 
type Props = {
  params: Promise<{ employeeId: string }>
  searchParams: Promise<{ [key: string]: string | string[] | undefined }>
}
 
export async function generateMetadata(
  { params, searchParams }: Props,
  parent: ResolvingMetadata
): Promise<Metadata> {
  // read route params
  const { employeeId: employeeId } = await params
 
  return {
    title: `AllHands - Employee ${employeeId}`,
  }
}

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <div>
      <h1>Employee by Id Page</h1>
        {children}
    </div>
  );
}
